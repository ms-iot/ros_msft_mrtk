// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if WINDOWS_UWP
#if !UNITY_2020_1_OR_NEWER
#define WLT_LEGACY_WSA
#elif WLT_MICROSOFT_OPENXR_PRESENT
#define WLT_SPATIAL_GRAPH_NODE
#endif // Legacy WSA
#endif // WINDOWS_UWP

using UnityEngine;

using Microsoft.MixedReality.WorldLocking.Tools;

/// A note about the appearance of "global::Windows.Etc" here. This is to disambiguate between
/// Windows.Perception.Spatial, which we want, and Microsoft.Windows.Perception.Spatial, which we don't want.
/// Because this sample is in a Microsoft namespace (Microsoft.MixedReality.WorldLocking.Samples), 
/// it will attempt to bind to the latter (Microsoft.Windows) unless the former is explicitly
/// indicated (global::Windows).

#if WLT_LEGACY_WSA
using global::Windows.Perception.Spatial;
#endif // WLT_LEGACY_WSA

#if WLT_MICROSOFT_OPENXR_PRESENT
using Microsoft.MixedReality.OpenXR;
#endif // WLT_MICROSOFT_OPENXR_PRESENT

/// <summary>
/// Wrapper class for SpatialCoordinateSystem.
/// </summary>
/// <remarks>
/// Provides a transform of the QR code's pose into Spongy space.
/// </remarks>
public class QRSpatialCoord
{

#if WLT_LEGACY_WSA
    /// <summary>
    /// Coordinate system of the QR Code.
    /// </summary>
    private SpatialCoordinateSystem coordinateSystem = null;

    /// <summary>
    /// Root coordinate system, aka Spongy space.
    /// </summary>
    private SpatialCoordinateSystem rootCoordinateSystem = null;
#endif // WLT_LEGACY_WSA

#if WLT_SPATIAL_GRAPH_NODE
    private SpatialGraphNode spatialGraphNode;
#endif // WLT_SPATIAL_GRAPH_NODE

    /// <summary>
    /// Spatial node id for the QR code.
    /// </summary>
    private System.Guid spatialNodeId;

    /// <summary>
    /// Accessor for spatial node id.
    /// </summary>
    public System.Guid SpatialNodeId
    {
        get { return spatialNodeId; }
        set
        {
            if (spatialNodeId != value)
            {
                spatialNodeId = value;
#if WLT_LEGACY_WSA
                coordinateSystem = null;
#endif // WLT_LEGACY_WSA
#if WLT_SPATIAL_GRAPH_NODE
                spatialGraphNode = null;
#endif // WLT_SPATIAL_GRAPH_NODE
            }
        }
    }

    // No error level logs currently used.
    //private static readonly int error = 10;

    /// <summary>
    /// The last computed pose.
    /// </summary>
    public Pose CurrentPose { get; private set; } = Pose.identity;

    /// <summary>
    /// Compute the head relative pose for the spatial node id.
    /// </summary>
    /// <param name="pose">If return value is true, the newly computed pose, else the last pose computed.</param>
    /// <returns>True if a new pose was successfully computed.</returns>
    /// <remarks>
    /// This ultimately relies on SpatialCoordinateSystem.TryGetTransformTo.
    /// TryGetTransformTo seems to fail for a while after the QR code is created. 
    /// Or maybe just spurious failure. Haven't found any documentation on behavior so far.
    /// Main thing is to be prepared for failure, and just try back until success.
    /// </remarks>
    public bool ComputePose(out Pose pose)
    {
        if (CheckActive())
        {
#if WLT_LEGACY_WSA
            if (UpdateCurrentPoseWSA())
            {
                pose = CurrentPose;
                return true;
            }
#endif // WLT_LEGACY_WSA
#if WLT_SPATIAL_GRAPH_NODE
            if (UpdateCurrentPoseGraphNode())
            {
                pose = CurrentPose;
                return true;
            }
#endif // WLT_SPATIAL_GRAPH_NODE
        }
        pose = CurrentPose;
        return false;
    }

#if WLT_LEGACY_WSA
    private bool UpdateCurrentPoseWSA()
    {
        System.Numerics.Matrix4x4? newMatrix = GetNewMatrix();
        if (newMatrix != null)
        {
            CurrentPose = AdjustNewMatrix(newMatrix.Value);
            return true;
        }
        return false;
    }
#endif // WLT_LEGACY_WSA

#if WLT_SPATIAL_GRAPH_NODE
    private bool UpdateCurrentPoseGraphNode()
    {
        if (spatialGraphNode.TryLocate(FrameTime.OnUpdate, out Pose pose))
        {
            CurrentPose = pose;
            return true;
        }
        return false;
    }
#endif // WLT_SPATIAL_GRAPH_NODE

#if WLT_LEGACY_WSA
    /// <summary>
    /// Attempt to retrieve the current transform matrix.
    /// </summary>
    /// <returns>Non-null matrix on success.</returns>
    private System.Numerics.Matrix4x4? GetNewMatrix()
    {
        Debug.Assert(rootCoordinateSystem != null);

        // Get the relative transform from the unity origin
        System.Numerics.Matrix4x4? newMatrix = coordinateSystem.TryGetTransformTo(rootCoordinateSystem);
        return newMatrix;
    }

    /// <summary>
    /// Convert the retrieved matrix to Unity lefthanded pose convention.
    /// </summary>
    /// <param name="newMatrix">Matrix to convert.</param>
    /// <returns>Unity pose equivalent.</returns>
    /// <remarks>
    /// Note that any scale is discarded, returned pose is position+rotation only.
    /// </remarks>
    private Pose AdjustNewMatrix(System.Numerics.Matrix4x4 newMatrix)
    {
        // Convert from right to left coordinate system
        newMatrix.M13 = -newMatrix.M13;
        newMatrix.M23 = -newMatrix.M23;
        newMatrix.M43 = -newMatrix.M43;

        newMatrix.M31 = -newMatrix.M31;
        newMatrix.M32 = -newMatrix.M32;
        newMatrix.M34 = -newMatrix.M34;

        /// Decompose into position + rotation (scale is discarded).
        System.Numerics.Vector3 sysScale;
        System.Numerics.Quaternion sysRotation;
        System.Numerics.Vector3 sysPosition;

        System.Numerics.Matrix4x4.Decompose(newMatrix, out sysScale, out sysRotation, out sysPosition);
        Vector3 position = new Vector3(sysPosition.X, sysPosition.Y, sysPosition.Z);
        Quaternion rotation = new Quaternion(sysRotation.X, sysRotation.Y, sysRotation.Z, sysRotation.W);
        Pose pose = new Pose(position, rotation);

        return pose;
    }
#endif // WLT_LEGACY_WSA

    /// <summary>
    /// Check that WorldManager is active and internal setup is cached.
    /// </summary>
    /// <returns></returns>
    private bool CheckActive()
    {
#if WLT_LEGACY_WSA
        if (UnityEngine.XR.WSA.WorldManager.state != UnityEngine.XR.WSA.PositionalLocatorState.Active)
        {
            return false;
        }
#endif // !WLT_LEGACY_WSA

#if WINDOWS_UWP
        if (!CheckCoordinateSystem())
        {
            return false;
        }
        return true;
#else // WINDOWS_UWP
        return false;
#endif // WINDOWS_UWP
    }

    /// <summary>
    /// Cache the coordinate system for the QR code's spatial node, and the root.
    /// </summary>
    /// <returns></returns>
    private bool CheckCoordinateSystem()
    {
#if WLT_LEGACY_WSA
        if (coordinateSystem == null)
        {
            coordinateSystem = global::Windows.Perception.Spatial.Preview.SpatialGraphInteropPreview.CreateCoordinateSystemForNode(SpatialNodeId);
        }

        if (rootCoordinateSystem == null)
        {
            rootCoordinateSystem = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(
                UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr()
            ) as SpatialCoordinateSystem;
        }

        return coordinateSystem != null;
#elif WLT_SPATIAL_GRAPH_NODE
        if (spatialGraphNode == null)
        {
            spatialGraphNode = SpatialGraphNode.FromStaticNodeId(SpatialNodeId);
        }
        return spatialGraphNode != null;
#else // WINDOWS_UWP
        return false;
#endif // WINDOWS_UWP
    }
}
