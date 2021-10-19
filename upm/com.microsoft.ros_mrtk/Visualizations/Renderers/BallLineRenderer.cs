using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sensor_msgs.msg;

public class BallLineRenderer : BallRenderer
{
    Material _material;

    public BallLineRenderer() : base()
    {
    }

    public override void Config(LidarVisualizer viz)
    {
        base.Config(viz);
        _material = _owner.ballLineMaterial;
    }

    public override void Render(LaserScan scan, Transform origin)
    {
        // Move the balls
        base.Render(scan, origin);

        if (_material != null)
        {
            for (int i = 0; i < _ballCache.Length; i++)
            {
                LineRenderer line = null;
                if (_ballCache[i].TryGetComponent<LineRenderer>(out line) == false)
                {
                    line = _ballCache[i].AddComponent<LineRenderer>() as LineRenderer;
                    line.material = _material;
                    line.endColor = new Color(0f, 0f, 0f, 0f);
                    line.startColor = new Color(.5f, 0f, 0f, .5f);
                    line.startWidth = 0.01f;
                    line.endWidth = 0.01f;
                }

                if (line != null)
                {
                    // wake up/activate the object if it wasn't used last frame
                    line.enabled = _ballCache[i].activeSelf;
                    line.SetPosition(0, _ballCache[i].transform.position);
                    line.SetPosition(1, origin.position);
                }
            }
        }
    }
}
