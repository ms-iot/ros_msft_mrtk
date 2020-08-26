using ROS2;
using RosSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelper 
{

    /// <summary>
    /// Convert ROS-spaced TfVector3 from TfListener to Unity-spaced Vector3
    /// </summary>
    /// <param name="vec">A TfVector3 as returned by the TransformListener class.</param>
    /// <returns>A vector3</returns>
    public static Vector3 VectorTfToUnity(TfVector3 vec)
    {
        return new Vector3((float)vec.y * -1, (float)vec.z, (float)vec.x);
    }

    /// <summary>
    /// Convert ROS-spaced TfQuaternion from TfListener to Unity-spaced Quaternion
    /// </summary>
    /// <param name="quat">A TfQuaternion as returned by the TransformListener class.</param>
    /// <returns>A quaternion</returns>
    public static Quaternion QuatTfToUnity(TfQuaternion quat)
    {
        return new Quaternion((float)quat.x, (float)quat.y, (float)quat.z, (float)quat.w);  // TASK: not yet implemented
    }

    /// <summary>
    /// Convert Apriltag-spaced Matd to Unity-spaced Vector3
    /// </summary>
    /// <param name="t">A matd containing a translation vector (NOT rotation)</param>
    /// <returns>A vector3?, which is null if the provided t was bad, otherwise the Unity-spaced vector3</returns>
    public static Vector3? VectorAprilToUnity(Matd t)
    {
        if (t.nrows * t.ncols != 3)  // xyz
        {
            Debug.LogError("Recieved a Matd of unexpected size! Expected 3 but got " + t.nrows * t.ncols);
            return null;
        }

        unsafe
        {
            return new Vector3((float)t.data[0], (float)t.data[1] * -1, (float)t.data[2]);
        }
    }

    /// <summary>
    /// Convert Apriltag-spaced Matd to Unity-spaced Quaternion
    /// </summary>
    /// <param name="R">A matd containing a rotation matrix (NOT translation)</param>
    /// <returns>A Quaternion?, which is null if the provided R was bad, otherwise the Unity-spaced quaternion</returns>
    public static Quaternion? QuatAprilToUnity(Matd R)
    {
        if (R.nrows * R.ncols != 9)  // rotation matrix
        {
            Debug.LogError("Recieved a Matd of unexpected size! Expected 9 but got " + R.nrows * R.ncols);
            return null;
        }

        unsafe
        {
            // First convert the rotation to Euler angles (easier to reason about and debug)
            // Math is from https://www.learnopencv.com/rotation-matrix-to-euler-angles/
            float sy = Mathf.Sqrt((float)R.data[0] * (float)R.data[0] + (float)R.data[3] * (float)R.data[3]);
            float x, y, z;
            if (sy < 1e-6)
            {
                x = Mathf.Atan2((float)R.data[7], (float)R.data[8]);
                y = Mathf.Atan2(-(float)R.data[6], sy);
                z = Mathf.Atan2((float)R.data[3], (float)R.data[0]);
            } else
            {
                x = Mathf.Atan2(-(float)R.data[5], (float)R.data[4]);
                y = Mathf.Atan2(-(float)R.data[6], sy);
                z = 0f;
            }

            Debug.Log(String.Format("Found an april rotation of <x:{0}, y:{1}, z:{2}>", x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg));

            // Finally, let the Unity Quaternion class handle conversion from Eulers to Quaternion
            return Quaternion.Euler(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg);
        }
    }
}
