using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROS2;
using System;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.WebCam;
using UnityEngine.XR.WSA;

public class FiducialSystem : MonoBehaviour
{
    [Tooltip("If true, write the webcam image, as seen by the Apriltag library, to the file specified in DEBUG_DUMP_IMAGE_NAME")]
    public bool DEBUG_DUMP_IMAGE;
    [Tooltip("The directory/name of the Apriltag debug dump image. Example: C:\\\\debugImg\\\\fooby.pnm")]
    public string DEBUG_DUMP_IMAGE_NAME;
    [Tooltip("In meters, how big the physical apriltags are.")]
    public double tagSize;

    public static FiducialSystem instance;

    private TransformListener _listener;
    private IntPtr _detector;
    private IntPtr _family;
    private ZArray _detections;
    private Intrensics _intrensics;
    private WorldAnchor _pinning;

    private bool _active = false;

    private struct RelTransform
    {
        public Vector3 translation;
        public Quaternion rotation;

        public RelTransform(Vector3 vec, Quaternion rot)
        {
            translation = vec;
            rotation = rot;
        }
        
    }


    /// <summary>
    /// Registers the OnSceneLoaded delegate early in game loop
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
#if UNITY_EDITOR
        string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
        string dllPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Plugins";
        if (currentPath.Contains(dllPath) == false)
        {
            Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath, EnvironmentVariableTarget.Process);
        }
#endif
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            RclCppDotnet.Init();
            _listener = new TransformListener();
            _detector = NativeFiducialFunctions.apriltag_detector_create();
            _family = NativeFiducialFunctions.tagStandard41h12_create();
            // Start detecting Apriltags from the 41h12 family:
            // https://github.com/AprilRobotics/apriltag-imgs/tree/master/tagStandard41h12
            NativeFiducialFunctions.apriltag_detector_add_family_bits(this._detector, this._family); 

            _active = true;
        } else
        {
            Debug.LogWarning("Duplicate FiducialSystem tried to initialize in scene on gameobject " + this.gameObject + "; Destroying self!");
            Destroy(this);
            return;
        }

        if (tagSize == 0)
        {
            Debug.LogError("Fiducial system is missing information about how large the physical AprilTag squares are!");
        }
    }

    /// <summary>
    /// Every time RobotScene is loaded, attempt to retrieve intrensics 
    /// and start updating the space pinning if successful
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("RobotScene"))
        {
            Intrensics? intr = CameraIntrensicsHelper.ReadIntrensics();
            if (intr.HasValue)
            {
                _intrensics = intr.Value;
                StartCoroutine("PinSpace");
            }
            else
            {
                Debug.Log("Failed to retrieve existing webcam parameters from disk. Manual calibration is required!");
                // Tell the UI to prompt user for calibration
                GoalPoseClient gpc = FindObjectOfType<GoalPoseClient>();
                if (gpc != null)
                {
                    gpc.State = GoalPoseClient.GoalPoseClientState.NEEDING_CALIBRATION;
                }
            }
        } else
        {
            CancelInvoke("PipePhotoToSpacePinning");
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == false)
        {
            this.Shutdown();
        }
#endif
    }

    private System.Object pinLock = new System.Object();
    private bool cameraInUse = false;

    /// <summary>
    /// Background thread that repeatedly queries WebcamSystem for images and pipes them
    /// to apriltag until an anchor is successfully created
    /// </summary>
    private IEnumerator PinSpace()
    {
        bool fin = false;
        // Keep taking pictures until UpdateSpacePinning pins the robot
        while (true)
        {
            // Wait for the FiducialSystem to finish initializing before trying to use it...
            if (!_active)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            
            // Only request another photo after the first request's callback is processed
            lock (pinLock)
            {
                if (!cameraInUse)
                {
                    cameraInUse = true;
                    // Pass anonymous function as callback which sets the currFrame on success
                    WebcamSystem.instance.CapturePhoto((PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame) =>
                    {
                        if (result.success && frame != null)
                        {
                            WebcamSystem.CaptureFrameInstance currFrame = null;
                            Debug.Log("cr snap!");
                            currFrame = new WebcamSystem.CaptureFrameInstance(frame);

                            // If the callback succeeded in this iteration, we can try to locate fiducials
                            // If the pin is successful, we can stop the coroutine
                            if (currFrame != null && UpdateSpacePinning(currFrame))
                            {
                                fin = true;
                            }

                        }
                        else
                        {
                            Debug.LogError("space pinning webcam call failed!");
                        }
                        cameraInUse = false;
                    });
                }
            }
            

            if (fin)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Pin successful");
    }

    private void OnApplicationQuit()
    {
        this.Shutdown();
    }

    /// <summary>
    /// Attempts to locate the ROS world origin, placing a world anchor at that location if found.
    /// </summary>
    /// <returns>
    /// true if a world anchor is created successfully, otherwise false.
    /// A world anchor could fail to be created for a number of reasons: webcam image did not have
    /// an identifiable apriltag, tf2 messages are not being found (no robot discovered, incorrect frame name)
    /// etc.
    /// </returns>
    private bool UpdateSpacePinning(WebcamSystem.CaptureFrameInstance captureFrame)
    {   

        if (_pinning != null)
        {
            return false;
        }

        if (DEBUG_DUMP_IMAGE)
        {
            Debug.LogWarning("dumping processed img...");
            int res = NativeFiducialFunctions.image_u8_write_pnm(captureFrame.unmanagedFrame, DEBUG_DUMP_IMAGE_NAME + "_" + System.DateTime.Now.Ticks + "_processed.pnm");
        }

        IntPtr nativeDetectionsHandle = NativeFiducialFunctions.apriltag_detector_detect(_detector, captureFrame.unmanagedFrame);

        _detections = Marshal.PtrToStructure<ZArray>(nativeDetectionsHandle);
        Dictionary<int, RelTransform> fiducialCentersRelWebcam = new Dictionary<int, RelTransform>();
        // Iterate over all detected apriltags in image
        for (int i = 0; i < _detections.size; i++)
        {
            IntPtr detPtr = IntPtr.Add(_detections.data, i * (int)_detections.el_sz);
            detPtr = Marshal.ReadIntPtr(detPtr);  // the zarray stores an array of detection ptrs
            ApriltagDetection det = Marshal.PtrToStructure<ApriltagDetection>(detPtr);
            Debug.Log("Detected an apriltag of id: " + det.id);

            AprilTagDetectionInfo info = new AprilTagDetectionInfo();
            info.det = detPtr;
            info.tagsize = tagSize;
            info.fx = _intrensics.fx;
            info.fy = _intrensics.fy;
            info.cx = _intrensics.cx;
            info.cy = _intrensics.cy;

            AprilTagPose pose = new AprilTagPose();
            double err = NativeFiducialFunctions.estimate_tag_pose(in info, out pose);


            Matd t = Marshal.PtrToStructure<Matd>(pose.t);
            Matd R = Marshal.PtrToStructure<Matd>(pose.R);
            
            Vector3? fiducialCenterRelWebcamTranslation = TransformHelper.VectorAprilToUnity(t);
            Quaternion? fiducialCenterRelWebcamRotation = TransformHelper.QuatAprilToUnity(R);

            if (fiducialCenterRelWebcamTranslation.HasValue && fiducialCenterRelWebcamRotation.HasValue)
            {
                fiducialCentersRelWebcam.Add(det.id, new RelTransform(fiducialCenterRelWebcamTranslation.Value,
                    fiducialCenterRelWebcamRotation.Value));
            } else
            {
                Debug.LogError("A TransformHelper apriltag conversion failed!");
            }
        }

        // Deallocate the Zarray of detections on native side
        NativeFiducialFunctions.apriltag_detections_destroy(nativeDetectionsHandle);
        nativeDetectionsHandle = IntPtr.Zero;

        // Use the fiducial tag labeled '1' to pin the anchor
        if (fiducialCentersRelWebcam.ContainsKey(1))
        {
            // Raw TF values follow a different xyz coordinate system, must be converted with the TransformHelper class
            TfVector3? worldZeroRelFiducialCenterTranslationTF = _listener.LookupTranslation("fiducial_link", "odom");
            TfQuaternion? worldZeroRelFiducialCenterRotationTF = _listener.LookupRotation("fiducial_link", "odom");

            if (worldZeroRelFiducialCenterTranslationTF.HasValue && worldZeroRelFiducialCenterRotationTF.HasValue)
            {
                Vector3 worldZeroRelFiducialCenterTranslation = TransformHelper.VectorTfToUnity(worldZeroRelFiducialCenterTranslationTF.Value);
                Quaternion worldZeroRelFiducialCenterRotation = TransformHelper.QuatTfToUnity(worldZeroRelFiducialCenterRotationTF.Value);
                // translate from camera pos -> fiducial tag pos
                Vector3 fiducialPos = Camera.main.transform.position + fiducialCentersRelWebcam[1].translation;
                // rotate from camera space -> fiducial space
                Quaternion fiducialRot = Camera.main.transform.rotation * fiducialCentersRelWebcam[1].rotation; 

                GameObject anchor = new GameObject("WorldZero");
                _pinning = anchor.AddComponent<WorldAnchor>();
                // first put anchor at fiducial location...
                anchor.transform.position = fiducialPos;
                anchor.transform.rotation = fiducialRot;
                // use fiducial location to utilize fiducial-spaced translation

                /*anchor.transform.position = anchor.transform.position + anchor.transform.right * worldZeroRelFiducialCenterTranslation.x
                    + anchor.transform.up * worldZeroRelFiducialCenterTranslation.y + anchor.transform.forward * worldZeroRelFiducialCenterTranslation.z;*/
                
                // end anchor positioning!
                Collocator.StartCollocation(_pinning);
                Debug.Log("Anchor laid at Unity-space coords" + anchor.transform.position);
                return true;
            } else
            {
                Debug.LogError("TF2 failed to query between fiducial tag and ROS world zero... is the ROS graph active?");
            }
        }
        
        return false; 
    }

    

    private void Shutdown()
    {
        if (_active)
        {
            RclCppDotnet.Shutdown();
            NativeFiducialFunctions.apriltag_detector_destroy(_detector);
            NativeFiducialFunctions.tagStandard41h12_destroy(_family);

            _active = false;
        }
    }
}
