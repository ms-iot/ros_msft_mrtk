using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;


public interface ISpaceRenderer
{
    /// <summary>
    /// Renders a frame of the visualization, cleaning up the previous frame if necessary
    /// </summary>
    void Render(float[] lidarData, Transform origin);
}

public enum SpaceRendererClass
{
    BALL,
    BALL_LINE,
    RING_MESH
}

public static class SpaceRenderer
{
    public static ISpaceRenderer GetSpaceRenderer(SpaceRendererClass src, GameObject owner)
    {
        switch (src)
        {
            case SpaceRendererClass.BALL:
                return new BallRenderer();  // TODO return new implementing class of proper type
            case SpaceRendererClass.BALL_LINE:
                return new BallLineRenderer();
            case SpaceRendererClass.RING_MESH:
                RingMeshRenderer rmr = owner.AddComponent<RingMeshRenderer>();
                return rmr;
        }
        Debug.LogError("Unsupported space renderer was asked for");
        return null;
    }
}
