using ROS2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorHelper 
{

    /// <summary>
    /// Convert ROS-spaced TfVector3 from TfListener to Unity-spaced Vector3
    /// </summary>
    /// <param name="vec">A TfVector3 as returned by the TransformListener class.</param>
    /// <returns>A vector3</returns>
    public static Vector3 TfToUnity(TfVector3 vec)
    {
        return new Vector3((float)vec.y * -1, (float)vec.z, (float)vec.x);
    }

    /// <summary>
    /// Convert Apriltag-spaced Matd to Unity-spaced Vector3
    /// </summary>
    /// <param name="t">A matd containing a transform (NOT rotation)</param>
    /// <returns>A vector3?, which is null if the provided t was bad, otherwise the Unity-spaced vector3</returns>
    public static Vector3? AprilToUnity(Matd t)
    {
        if (t.nrows * t.ncols != 3)  // xyz
        {
            Debug.LogError("Recieved a Matd of unexpected size! Expected 3 but got " + t.nrows * t.ncols);
            return null;
        }

        Vector3? output = null;
        unsafe
        {
            output = new Vector3((float)t.data[0], (float)t.data[1] * -1, (float)t.data[3]);
        }

        return output;
    }
}
