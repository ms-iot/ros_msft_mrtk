using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ROS2;
using System;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

public class FiducialSystem : MonoBehaviour
{
    public bool DEBUG_DUMP_IMAGE;

    public static FiducialSystem instance;

    private TransformListener listener;
    private IntPtr detector;
    private IntPtr family;
    private ZArray detections;
    private Intrensics intrensics;

    private bool active = false;


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
    void Start()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            RclCppDotnet.Init();
            this.listener = new TransformListener();
            this.detector = NativeFiducialFunctions.apriltag_detector_create();
            this.family = NativeFiducialFunctions.tagStandard41h12_create();

            active = true;

            Intrensics? intr = CameraIntrensicsHelper.ReadIntrensics();
            if (intr.HasValue)
            {
                intrensics = intr.Value;
            } else
            {
                Debug.Log("Failed to retrieve existing webcam parameters from disk. Manual calibration is required!");
                GoalPoseClient gpc = FindObjectOfType<GoalPoseClient>();
                if (gpc != null)
                {
                    gpc.State = GoalPoseClient.GoalPoseClientState.NEEDING_CALIBRATION;
                }
            }
        } else
        {
            Debug.LogWarning("Duplicate FiducialSystem tried to initialize in scene on gameobject " + this.gameObject + "; Destroying self!");
            Destroy(this);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == false)
        {
            this.Shutdown();
        }
#endif
    }

    private void OnApplicationQuit()
    {
        this.Shutdown();
    }

    public void UpdateSpacePinning(WebcamSystem.CaptureFrameInstance captureFrame)
    {

        TfVector3? loc = listener.LookupTranslation("base_link", "map");

        if (loc != null)
        {
            Debug.Log(string.Format("Location in map frame is currently: {0}, {1}, {2}", loc.Value.x, loc.Value.y, loc.Value.z));
        }
        
        if (DEBUG_DUMP_IMAGE)
        {
            int res = NativeFiducialFunctions.image_u8_write_pnm(captureFrame.unmanagedFrame, "m:\\debugImg\\garboogle.pnm");
        }

        IntPtr nativeDetectionsHandle = NativeFiducialFunctions.apriltag_detector_detect(detector, captureFrame.unmanagedFrame);

        detections = Marshal.PtrToStructure<ZArray>(nativeDetectionsHandle);

        for (int i = 0; i < detections.size; i++)
        {
            IntPtr det = IntPtr.Add(detections.data, i * Marshal.SizeOf<ApriltagDetection>());
            AprilTagDetectionInfo info = new AprilTagDetectionInfo();
            info.det = det;
            //info.fx = blah
            //info.fy = blah
            //info.cx = blah
            //info.cy = blah
            AprilTagPose pose = new AprilTagPose();
            double err = NativeFiducialFunctions.estimate_tag_pose(in info, out pose);
        }

        NativeFiducialFunctions.apriltag_detections_destroy(nativeDetectionsHandle);

        nativeDetectionsHandle = IntPtr.Zero;
    }

    

    private void Shutdown()
    {
        if (active)
        {
            RclCppDotnet.Shutdown();
            NativeFiducialFunctions.apriltag_detector_destroy(detector);
            NativeFiducialFunctions.tagStandard41h12_destroy(family);

            active = false;
        }
    }
}
