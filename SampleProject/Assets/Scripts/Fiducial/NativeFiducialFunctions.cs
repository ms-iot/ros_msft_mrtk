using ROS2.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Contains numerous native C function definitions
/// to support the fiducial-related features needed
/// to perform space pinning.
/// </summary>
public static class NativeFiducialFunctions
{
    #region Apriltag P/Invoke

    /// <summary>
    /// Creates an apriltag detector object
    /// </summary>
    /// <returns>The apriltag detector handle. You are responsible for deallocating this with apriltag_detector_destroy</returns>
    [DllImport("apriltags-umich")]
    public static extern IntPtr apriltag_detector_create();

    /// <summary>
    /// Deallocates an apriltag detector object
    /// </summary>
    /// <param name="detector">the detector to be deallocated</param>
    [DllImport("apriltags-umich")]
    public static extern void apriltag_detector_destroy(IntPtr detector);

    /// <summary>
    /// Creates the 41h12 library (a set of fiducial tags apriltag will look for)
    /// </summary>
    /// <returns>the library handle. You are responsible for deallocating this with tagStandard41h12_destroy</returns>
    [DllImport("apriltags-umich")]
    public static extern IntPtr tagStandard41h12_create();

    /// <summary>
    /// Deallocates the 41h12 library
    /// </summary>
    /// <param name="family">the library to be deallocated</param>
    [DllImport("apriltags-umich")]
    public static extern void tagStandard41h12_destroy(IntPtr family);

    /// <summary>
    /// Tells a given apriltag detector to search for all tags found in the given library/family
    /// </summary>
    /// <param name="detector">The detector</param>
    /// <param name="family">The tag library to be added to detector</param>
    /// <param name="bits_corrected">Default 2. Should not be changed unless you understand the apriltag algorithm.</param>
    [DllImport("apriltags-umich")]
    public static extern void apriltag_detector_add_family_bits(IntPtr detector, IntPtr family, int bits_corrected = 2);

    /// <summary>
    /// Attempt to detect any tags registered with the given detector.
    /// </summary>
    /// <param name="detector">
    /// An apriltag detector, registered with the tag libraries you are attempting to find.
    /// Will not do anything without first calling tagStandard41h12_create and apriltag_detector_add_family_bits
    /// to register the tag library.
    /// </param>
    /// <param name="img">An image_u8 to be searched for an apriltag</param>
    /// <returns>
    /// A ZArray of ApriltagDetections. You are responsible for deallocation
    /// of this construct using apriltag_detections_destroy.
    /// </returns>
    [DllImport("apriltags-umich")]
    public static extern IntPtr apriltag_detector_detect(IntPtr detector, IntPtr img);

    /// <summary>
    /// Deallocates a ZArray of ApriltagDetections
    /// </summary>
    /// <param name="detections"></param>
    [DllImport("apriltags-umich")]
    public static extern void apriltag_detections_destroy(IntPtr detections);

    /// <summary>
    /// Attempts to estimate the pose of a detected tag relative to the camera (source of image).
    /// </summary>
    /// <param name="info">A struct, populated with information about the camera and tag size</param>
    /// <param name="pose">An output variable which will be populated with the pose data if successful</param>
    [DllImport("apriltags-umich")]
    public static extern double estimate_tag_pose(in AprilTagDetectionInfo info, out AprilTagPose pose);


    // debug
    [DllImport("apriltags-umich")]
    public static extern int image_u8_write_pnm(IntPtr image, string s);


    #endregion // Apriltag P/Invoke


    #region Calibration P/Invoke
    /// <summary>
    /// Adds an image to the calibration buffer.
    /// </summary>
    /// <param name="img">image_u8</param>
    /// <returns>The number of images now stored in the calibration buffer.</returns>
    [DllImport("opencv-c-wrapper")]
    public static extern int supply_calibration_image(IntPtr img);

    /// <summary>
    /// Clears the calibration buffer.
    /// </summary>
    /// <returns>The number of images now stored in the calibration buffer.</returns>
    [DllImport("opencv-c-wrapper")]
    public static extern int clear_calibration_images();

    /// <summary>
    /// Used all images stored in the calibration buffer to perform camera calibration.
    /// </summary>
    /// <param name="squareSize">How large, in meters, a single square on the physical checkerboard pattern is.</param>
    /// <param name="intrensics">Output variable providing the calculated camera intrinsics.</param>
    /// <returns></returns>
    [DllImport("opencv-c-wrapper")]
    public static extern int calibrate(float squareSize, out Intrensics intrensics);

    #endregion  // Calibration P/Invoke

}
