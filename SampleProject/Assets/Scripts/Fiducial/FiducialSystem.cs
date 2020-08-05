using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ROS2;
using System;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using System.Text.RegularExpressions;

public class FiducialSystem : MonoBehaviour
{
    #region Apriltag P/Invoke

    [DllImport("apriltags-umich")]
    private static extern IntPtr apriltag_detector_create();

    [DllImport("apriltags-umich")]
    private static extern void apriltag_detector_destroy(IntPtr detector);

    [DllImport("apriltags-umich")]
    private static extern IntPtr tagStandard41h12_create();

    [DllImport("apriltags-umich")]
    private static extern void tagStandard41h12_destroy(IntPtr family);

    [DllImport("apriltags-umich")]
    private static extern void apriltag_detector_add_family(IntPtr detector, IntPtr family);

    [DllImport("apriltags-umich")]
    private static extern IntPtr apriltag_detector_detect(IntPtr detector, IntPtr img);

    [DllImport("apriltags-umich")]
    private static extern void apriltag_detections_destroy(IntPtr detections);

    [DllImport("apriltags-umich")]
    private static extern double estimate_tag_pose(in AprilTagDetectionInfo info, out AprilTagPose pose);

    #endregion // Apriltag P/Invoke


    #region Apriltag Structures

    [StructLayout(LayoutKind.Sequential)]
    private struct ZArray
    {
        public ulong el_sz;
        public int size;
        public int alloc;
        public IntPtr data;
    }

    
    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct ApriltagDetection
    {
        public IntPtr family;
        public int id;
        public int hamming;
        public float decision_margin;

        public Matd* H;

        public fixed double c[2];
        public fixed double p[8];
    } 

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct Matd
    {
        public uint nrows;
        public uint ncols;
        public double* data;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AprilTagDetectionInfo
    {
        public IntPtr det;
        public double tagsize;
        public double fx;
        public double fy;
        public double cx;
        public double cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct AprilTagPose
    {
        public Matd* R;
        public Matd* t;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct image_u8
    {
        public int width;
        public int height;
        public int stride;

        public IntPtr buf;
    }

    #endregion // Apriltag Structures

    private static FiducialSystem instance;

    private TransformListener listener;

    private IntPtr detector;
    private IntPtr family;
    private ZArray detections;

    // Start is called before the first frame update
    void Start()
    {
        
        if (instance == null)
        {
            instance = this;
            RclCppDotnet.Init();
            this.listener = new TransformListener();
            detector = apriltag_detector_create();
            family = tagStandard41h12_create();
        } else
        {
            Debug.LogWarning("Duplicate FiducialSystem tried to initialize in scene on gameobject " + this.gameObject + "; Destroying self!");
            Destroy(this);
        }
    }

    public void UpdateSpacePinning(WebcamSystem.CaptureFrameInstance captureFrame)
    {
        TfVector3? loc = listener.LookupTranslation("base_link", "map");
        if (loc != null)
        {
            Debug.Log(string.Format("Location in map frame is currently: {0}, {1}, {2}", loc.Value.x, loc.Value.y, loc.Value.z));
        }
        // TODO replace IntPtr.Zero with an actual img
        IntPtr nativeDetectionsHandle = apriltag_detector_detect(detector, &captureFrame);

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
            double err = estimate_tag_pose(in info, out pose);

        }
        apriltag_detections_destroy(nativeDetectionsHandle);
        nativeDetectionsHandle = IntPtr.Zero;
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

    private void Shutdown()
    {
        RclCppDotnet.Shutdown();
        apriltag_detector_destroy(detector);
        tagStandard41h12_destroy(family);
    }
}
