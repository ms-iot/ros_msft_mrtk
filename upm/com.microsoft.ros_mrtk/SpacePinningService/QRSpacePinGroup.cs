// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using ROS2;
using System.Linq;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.WorldLocking.Core;
using Microsoft.MixedReality.WorldLocking.Tools;
using Microsoft.MixedReality.QR;

/// <summary>
/// A group of space pins with locations fed by reading QR codes placed in the physical environment.
/// </summary>
public class QRSpacePinGroup : MonoBehaviour
{
    /// <summary>
    /// An orienter to infer orientation from position of pins. Shared over all pins.
    /// </summary>
    private IOrienter orienter;

    [SerializeField]
    [Tooltip("An orienter instance in the scene. If unset, one will be created.")]
    private Orienter sharedOrienter = null;

    /// <summary>
    /// An orienter instance in the scene. If unset, one will be created.
    /// </summary>
    public Orienter SharedOrienter { get { return sharedOrienter; } set { sharedOrienter = value; orienter = value; } }

    [SerializeField]
    [Tooltip("Optional visual to draw at QR code position when detected.")]
    private GameObject markerHighlightPrefab;

    /// <summary>
    /// Optional visual to draw at QR code position when detected.
    /// </summary>
    public GameObject MarkerHighlightPrefab { get { return markerHighlightPrefab; } set { markerHighlightPrefab = value; } }

    /// <summary>
    /// Whether the enumeration completed event has been encountered yet.
    /// </summary>
    /// <remarks>
    /// Added, Updated, and Removed events before enumerationFinished is true are cached,
    /// not seen in the current session. These are ignored in favor of the SpacePin persistence
    /// system instead.
    /// </remarks>
    private bool enumerationFinished = false;

    // Error level log currently unused.
    //private static readonly int error = 10;

    /// <summary>
    /// A collection of everything needed to set a space pin from a detected QR code.
    /// </summary>
    private class SpacePinPackage
    {
        /// <summary>
        /// The space pin to generate and maintain.
        /// </summary>
        public SpacePinOrientable spacePin = null;

        /// <summary>
        /// Optional visual to instantiate at QR code's position.
        /// </summary>
        private GameObject highlightPrefab = null;

        /// <summary>
        /// Instantiated visual for the QR code's detected position and orientation.
        /// </summary>
        public GameObject highlightProxy = null;

        /// <summary>
        /// Helper class to transform QR code's pose into Unity's global space.
        /// </summary>
        public QRSpatialCoord coordinateSystem = null;

        /// <summary>
        /// Size of the detected QR code. Only used for visualization.
        /// </summary>
        private float sizeMeters = 1.0f;

        /// <summary>
        /// The locked pose last sent to WLT space pin. 
        /// </summary>
        /// <remarks>
        /// This is only used to detect when the read QR position has changed enough to resubmit.
        /// </remarks>
        private Pose lastLockedPose = Pose.identity;

        /// <summary>
        /// Whether the QR code pose has been submitted to the space pin yet.
        /// </summary>
        private bool isSet = false;

        /// <summary>
        /// Create a new space pin package.
        /// </summary>
        /// <param name="owner">The owning space pin group.</param>
        /// <param name="virtualObject">Corresponding virtual object (for pose) in the scene.</param>
        /// <returns>The created package.</returns>
        /// <remarks>
        /// The created space pin package is ready to deploy, but currently idle.
        /// </remarks>
        public static SpacePinPackage Create(QRSpacePinGroup owner, Transform virtualObject)
        {
            SpacePinPackage package = new SpacePinPackage();
            package.spacePin = virtualObject.gameObject.AddComponent<SpacePinOrientable>();
            package.spacePin.Orienter = owner.orienter;
            package.highlightProxy = null;
            package.highlightPrefab = owner.markerHighlightPrefab;

            package.coordinateSystem = new QRSpatialCoord();

            return package;
        }

        /// <summary>
        /// Release all resources. Package is unusable after Release.
        /// </summary>
        public void Release()
        {
            Destroy(spacePin);
            spacePin = null;
            Destroy(highlightProxy);
            highlightProxy = null;
        }

        /// <summary>
        /// Reset package to initial state. If space pin has been committed, it will be rescinded.
        /// </summary>
        public void Reset()
        {
            spacePin.Reset();
            Destroy(highlightProxy);
            highlightProxy = null;
            isSet = false;
        }

        /// <summary>
        /// Attempt to set a space pin from the QR code.
        /// </summary>
        /// <param name="qrCode">The source QR code.</param>
        /// <returns>True if a space pin was set from the current data.</returns>
        /// <remarks>
        /// Returning false does not necessarily mean an error occurred. For example, if the space pin
        /// has already been set from the given QR code, and the location hasn't changed, no action
        /// will be taken and the return value will be false. Or if the coordinate system is unable
        /// to resolve the transform to global space, again the return will be false, indicating
        /// trying again later. 
        /// </remarks>
        public bool Update(QRCode qrCode)
        {
            coordinateSystem.SpatialNodeId = qrCode.SpatialGraphNodeId;
            sizeMeters = qrCode.PhysicalSideLength;
            Pose spongyPose;
            if (!coordinateSystem.ComputePose(out spongyPose))
            {
                return false;
            }
            Pose frozenPose = WorldLockingManager.GetInstance().FrozenFromSpongy.Multiply(spongyPose);
            return UpdatePose(frozenPose);
        }

        /// <summary>
        /// Given a new frozen pose, send it to SpacePin system if appropriate.
        /// </summary>
        /// <param name="frozenPose">New frozen space pose.</param>
        /// <returns>True if the pose was pushed to the SpacePin system.</returns>
        private bool UpdatePose(Pose frozenPose)
        {
            bool didCommit = false;
            var wltMgr = WorldLockingManager.GetInstance();
            Pose lockedPose = wltMgr.LockedFromFrozen.Multiply(frozenPose);
            if (NeedCommit(lockedPose))
            {
                didCommit = CommitPose(frozenPose, lockedPose);
            }
            else if (highlightProxy != null)
            {
                if (!highlightProxy.activeSelf)
                {
                    // Proxy has deactivated itself at end of animation, go ahead and destroy it.
                    Destroy(highlightProxy);
                    highlightProxy = null;
                }
            }
            return didCommit;
        }

        /// <summary>
        /// Commit the pose to the SpacePin system, deploying the highlight marker if one is specified.
        /// </summary>
        /// <param name="frozenPose">New pose in frozen space.</param>
        /// <param name="lockedPose">New pose in locked space.</param>
        /// <returns>True if pose successfully committed to SpacePin system.</returns>
        private bool CommitPose(Pose frozenPose, Pose lockedPose)
        {
            spacePin.SetFrozenPose(frozenPose);

            DeployProxy();

            isSet = true;
            lastLockedPose = lockedPose;
            return true;
        }

        /// <summary>
        /// If a prefab for the highlight marker has been specified, instantiate it where the QR code was seen.
        /// </summary>
        /// <param name="frozenPose">Pose in frozen space where the qr code was seen.</param>
        /// <returns>True if the visual was deployed.</returns>
        private bool DeployProxy()
        {
            if (highlightProxy != null)
            {
                Destroy(highlightProxy);
            }
            if (highlightPrefab != null)
            {
                /// Note the assumption that the QR proxy is a box that needs to be
                /// sized and offset to fit the read QR data.
                Vector3 scale = new Vector3(sizeMeters, sizeMeters, sizeMeters * 0.1f);
                Vector3 offset = scale * 0.5f;
                highlightProxy = Instantiate(highlightPrefab, spacePin.transform);
                highlightProxy.transform.localScale = scale;
                highlightProxy.transform.localPosition = offset;
                highlightProxy.transform.localRotation = Quaternion.identity;
            }

            return highlightProxy != null;
        }

        /// <summary>
        /// Determine if the new pose should be forwarded to the SpacePin system.
        /// </summary>
        /// <param name="lockedPose">The pose to test.</param>
        /// <returns>True if sending the new pose is indicated.</returns>
        /// <remarks>
        /// If the pose hasn't been sent yet, it will always be indicated to send.
        /// It is unusual that the position of the QR code as measured by the system changes
        /// significantly enough to be worth resending. It usually only occurs when the first 
        /// reading was faulty (e.g. during a rapid head move).
        /// </remarks>
        private bool NeedCommit(Pose lockedPose)
        {
            if (!isSet)
            {
                return true;
            }
            float RefreshThreshold = 0.01f; // one cm?
            float distance = Vector3.Distance(lockedPose.position, lastLockedPose.position);
            if ( distance > RefreshThreshold)
            {
                return true;
            }
            return false;
        }
    };

    /// <summary>
    /// One pin created for each spacePinPoint.
    /// </summary>
    private Dictionary<string, SpacePinPackage> spacePins = new Dictionary<string, SpacePinPackage>();

    private ISpatialPinningService spacePinningService;

    /// <summary>
    /// Ensure all required components exist and cache references where appropriate.
    /// </summary>
    private void CheckComponents()
    {
        if (spacePinningService == null)
        {
            spacePinningService = MixedRealityToolkit.Instance.GetService<ISpatialPinningService>();
            spacePinningService?.Enable();
        }
        if (orienter == null)
        {
            if (sharedOrienter == null)
            {
                orienter = gameObject.AddComponent<Orienter>();
            }
            else
            {
                orienter = sharedOrienter;
            }
        }
    }

    /// <summary>
    /// Prepare to activate.
    /// </summary>
    private void Start()
    {
        CheckComponents();
    }

    /// <summary>
    /// Become active.
    /// </summary>
    private void OnEnable()
    {
        CheckComponents();

        SetUpCallbacks();
    }

    /// <summary>
    /// Free all resources created to support the space pins.
    /// </summary>
    /// <remarks>
    /// After calling this, the group is no longer useable.
    /// </remarks>
    private void DestroySpacePins()
    {
        foreach (var pin in spacePins)
        {
            pin.Value.Release();
        }
        spacePins.Clear();
    }

    /// <summary>
    /// Go into dormant state. Can be revived later.
    /// </summary>
    private void OnDisable()
    {
        TearDownCallbacks();
    }

    /// <summary>
    /// Release resources in prepartion for destruction. Cannot be revived.
    /// </summary>
    private void OnDestroy()
    {
        DestroySpacePins();
    }

    /// <summary>
    /// Register for callbacks on QR code events. These callbacks will happen on the main thread.
    /// </summary>
    private void SetUpCallbacks()
    {
        if (spacePinningService != null)
        {
            spacePinningService.OnQRAdded += OnQRCodeAdded;
            spacePinningService.OnQRUpdated += OnQRCodeUpdated;
            spacePinningService.OnQRRemoved += OnQRCodeRemoved;
            spacePinningService.OnQREnumerated += OnQRCodeEnumerated;
        }
    }

    /// <summary>
    /// Unregister from callbacks.
    /// </summary>
    private void TearDownCallbacks()
    {
        if (spacePinningService != null)
        {
            spacePinningService.OnQRAdded -= OnQRCodeAdded;
            spacePinningService.OnQRUpdated -= OnQRCodeUpdated;
            spacePinningService.OnQRRemoved -= OnQRCodeRemoved;
            spacePinningService.OnQREnumerated -= OnQRCodeEnumerated;
            spacePinningService = null;
        }
    }

    /// <summary>
    /// Process a newly added QR code.
    /// </summary>
    /// <param name="qrCode">The qr code to process.</param>
    private void OnQRCodeAdded(QRCode qrCode)
    {
        string frameId;
        if (ExtractFrameId(qrCode, out frameId))
        {
            if (spacePins.ContainsKey(frameId) == false)
            {
                Vector3 vecU = Vector3.zero;
                Quaternion quatU = Quaternion.identity;
                TfVector3? vecTF = spacePinningService.SharedListener.LookupTranslation("map", frameId);
                TfQuaternion? quatTF = spacePinningService.SharedListener.LookupRotation("map", frameId);

                if (vecTF.HasValue == false || quatTF.HasValue == false)
                {
                    Debug.LogWarning($"Could not find a ROS transform from map to {frameId}; assuming map == frameId");
                }
                else
                {
                    vecU = TransformHelper.VectorTfToUnity(vecTF.Value);
                    quatU = TransformHelper.QuatTfToUnity(quatTF.Value);

                    if (spacePinningService.UseCenterOfQRCode)
                    {
                        // QRCode zero is at the top left corner; but for mounting center can be better.
                        Vector3 qrCenterOffset = new Vector3(qrCode.PhysicalSideLength / 2.0f, qrCode.PhysicalSideLength / 2.0f, 0.0f);
                        vecU += qrCenterOffset;
                    }

                    Quaternion correction = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                    quatU = quatU * correction;
                }

                var qrCodeLocationInROSSpace = new GameObject(frameId);
                    qrCodeLocationInROSSpace.transform.parent = this.transform;

                qrCodeLocationInROSSpace.transform.SetPositionAndRotation(vecU, quatU);

                spacePins[frameId] = SpacePinPackage.Create(this, qrCodeLocationInROSSpace.transform);
            }
        }
    }

    /// <summary>
    /// Process a newly updated QR code.
    /// </summary>
    /// <param name="qrCode">The qr code to process.</param>
    private void OnQRCodeUpdated(QRCode qrCode)
    {
        if (enumerationFinished)
        {
            string frameId;
            if (ExtractFrameId(qrCode, out frameId) && spacePins.ContainsKey(frameId))
            {
                spacePins[frameId].Update(qrCode);
            }
        }
    }

    /// <summary>
    /// Process a newly removed QR code.
    /// </summary>
    /// <param name="qrCode">The qr code to process.</param>
    private void OnQRCodeRemoved(QRCode qrCode)
    {
        string frameId;
        if (ExtractFrameId(qrCode, out frameId) && spacePins.ContainsKey(frameId))
        {
            spacePins[frameId].Reset();
            spacePins.Remove(frameId);
        }
    }

    /// <summary>
    /// Process the enumeration completed event.
    /// </summary>
    /// <param name="qrCode"></param>
    private void OnQRCodeEnumerated(QRCode qrCode)
    {
        Debug.Assert(qrCode == null, "Dummy qrCode parameter should always be null");
        enumerationFinished = true;
    }

    /// <summary>
    /// Extract a space pin index out of the qr code.
    /// </summary>
    /// <param name="qrCode"></param>
    /// <returns>Space pin index corresponding to this qr code, or -1 if there isn't one.</returns>
    private bool ExtractFrameId(QRCode qrCode, out string frameId)
    {
        string uri = qrCode.Data;
        System.Uri rosUri;
        frameId = string.Empty;

        if (System.Uri.TryCreate(uri, System.UriKind.Absolute, out rosUri))
        {
            // Quick test to see if this is a ROS QRCode
            if (rosUri.AbsoluteUri.StartsWith(spacePinningService.URIPrefix))
            {
                // var queryParameters = System.Web.HttpUtility.ParseQueryString(rosUri.Query); // System.Web.Utilities is not available in unity directly.

                var queryParameters = rosUri.Query
                    .Substring(1)   // Remove the question mark
                    .Split('&')     // split result into key=value pairs
                    .Select(queryParams => queryParams.Split('='))  // split into key & value
                    .ToDictionary(  // turn into dictionary
                            queryParam => queryParam.FirstOrDefault(), // Key
                            queryParam => queryParam.Skip(1).FirstOrDefault());    // value

                // if no frameId specified, use the system default
                if (queryParameters.TryGetValue("frame", out frameId) == false||
                    string.IsNullOrEmpty(frameId))
                {
                    frameId = spacePinningService.DefaultFrameId;
                }

                return true;
            }
        }

        return false;
    }
}
