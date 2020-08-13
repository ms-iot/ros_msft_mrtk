using ROS2.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class NativeFiducialFunctions
{
    #region Apriltag P/Invoke

    [DllImport("apriltags-umich")]
    public static extern IntPtr apriltag_detector_create();

    [DllImport("apriltags-umich")]
    public static extern void apriltag_detector_destroy(IntPtr detector);

    [DllImport("apriltags-umich")]
    public static extern IntPtr tagStandard41h12_create();

    [DllImport("apriltags-umich")]
    public static extern void tagStandard41h12_destroy(IntPtr family);

    [DllImport("apriltags-umich")]
    public static extern void apriltag_detector_add_family(IntPtr detector, IntPtr family);

    [DllImport("apriltags-umich")]
    public static extern IntPtr apriltag_detector_detect(IntPtr detector, IntPtr img);

    [DllImport("apriltags-umich")]
    public static extern void apriltag_detections_destroy(IntPtr detections);

    [DllImport("apriltags-umich")]
    public static extern double estimate_tag_pose(in AprilTagDetectionInfo info, out AprilTagPose pose);


    // debug
    [DllImport("apriltags-umich")]
    public static extern int image_u8_write_pnm(IntPtr image, string s);


    #endregion // Apriltag P/Invoke


    #region Calibration P/Invoke

    [DllImport("ros-msft-mrtk-native")]
    public static extern int supply_calibration_image(IntPtr img);

    [DllImport("ros-msft-mrtk-native")]
    public static extern int clear_calibration_images();

    [DllImport("ros-msft-mrtk-native")]
    public static extern int calibrate(float squareSize, out Intrensics intrensics);

    #endregion  // Calibration P/Invoke

}
