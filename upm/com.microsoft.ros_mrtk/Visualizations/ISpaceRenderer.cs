using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sensor_msgs.msg;

public interface ISpaceRenderer
{
    /// <summary>
    /// Renders a frame of the visualization, cleaning up the previous frame if necessary
    /// </summary>
    void Render(LaserScan scan, Transform origin);
    void Config(LidarVisualizer viz);
}


public enum SpaceRendererClass
{
    BALL,
    BALL_LINE,
    RING_MESH
}

public static class SpaceRenderer
{
    public static ISpaceRenderer GetSpaceRenderer(SpaceRendererClass src, LidarVisualizer owner)
    {
        ISpaceRenderer renderer = null;
        switch (src)
        {
            case SpaceRendererClass.BALL:
                renderer = new BallRenderer();  // TODO return new implementing class of proper type
                break;
            case SpaceRendererClass.BALL_LINE:
                renderer = new BallLineRenderer();
                break;
            case SpaceRendererClass.RING_MESH:
                renderer = owner.gameObject.AddComponent<RingMeshRenderer>();
                break;
        }
        if (renderer == null)
        {
            Debug.LogError("Unsupported space renderer was asked for");
        }
        renderer.Config(owner);
        return renderer;
    }
}