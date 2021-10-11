// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

using Microsoft.MixedReality.WorldLocking.Tools;
#if WINDOWS_UWP
using Microsoft.MixedReality.OpenXR;
#endif

/// <summary>
/// Wrapper class for SpatialCoordinateSystem.
/// </summary>
/// <remarks>
/// Provides a transform of the QR code's pose into Spongy space.
/// </remarks>
public class QRSpatialCoord
{
#if WINDOWS_UWP
    private SpatialGraphNode spatialGraphNode;
#endif
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
#if WINDOWS_UWP
                spatialGraphNode = null;
#endif
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
            if (UpdateCurrentPoseGraphNode())
            {
                pose = CurrentPose;
                return true;
            }
        }
        pose = CurrentPose;
        return false;
    }

    private bool UpdateCurrentPoseGraphNode()
    {
#if WINDOWS_UWP
        if (spatialGraphNode.TryLocate(FrameTime.OnUpdate, out Pose pose))
        {
            CurrentPose = pose;
            return true;
        }
#endif
        return false;
    }

    /// <summary>
    /// Check that WorldManager is active and internal setup is cached.
    /// </summary>
    /// <returns></returns>
    private bool CheckActive()
    {
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
#if WINDOWS_UWP
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
