using ROS2;
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
        return new Vector3(-(float)vec.x, (float)vec.z, (float)vec.y);
    }

    /// <summary>
    /// Convert ROS-spaced TfQuaternion from TfListener to Unity-spaced Quaternion
    /// </summary>
    /// <param name="quat">A TfQuaternion as returned by the TransformListener class.</param>
    /// <returns>A quaternion</returns>
    public static Quaternion QuatTfToUnity(TfQuaternion quat)
    {
        Quaternion tfQ = new Quaternion(-(float)quat.x, -(float)quat.z, -(float)quat.y, (float)quat.w);
        // Right hand to Left Hand
        return tfQ;
    }
}
