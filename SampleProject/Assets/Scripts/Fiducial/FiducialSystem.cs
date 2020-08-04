using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ROS2;
using System;
using System.Runtime.InteropServices;

public class FiducialSystem : MonoBehaviour
{
    private static FiducialSystem instance;

    private TransformListener listener;

    #region Apriltag P/Invoke
    [DllImport("apriltags-umich")]
    private static extern IntPtr apriltag_detector_create();

    [DllImport("apriltags-umich")]
    private static extern void apriltag_detector_destroy(IntPtr det);
    #endregion // Apriltag P/Invoke

    // Start is called before the first frame update
    void Start()
    {
        IntPtr ptr = apriltag_detector_create();
        apriltag_detector_destroy(ptr);
        if (instance == null)
        {
            RclCppDotnet.Init();
            this.listener = new TransformListener();
        } else
        {
            Debug.LogWarning("Duplicate FiducialSystem tried to initialize in scene on gameobject " + this.gameObject + "; Destroying self!");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TfVector3? loc = listener.LookupTranslation("base_link", "map");
        if (loc != null)
        {
            Debug.Log(string.Format("Location in map frame is currently: {0}, {1}, {2}", loc.Value.x, loc.Value.y, loc.Value.z));
        }

#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == false)
        {
            RclCppDotnet.Shutdown();
        }
#endif
    }

    private void OnApplicationQuit()
    {
        RclCppDotnet.Shutdown();
    }
}
