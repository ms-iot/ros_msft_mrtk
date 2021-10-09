using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLineRenderer : BallRenderer
{
    const string _lidarMaterialName = "LidarLine.mat";
    Material _material;

    public BallLineRenderer() : base()
    {
        _material = Resources.Load<Material>(_lidarMaterialName);
        if (_material == null)
        {
            Debug.LogError($"BallLineRenderer failed to locate the {_lidarMaterialName} material!");
        }

    }

    public override void Render(float[] lidarData, Transform origin)
    {
        // Move the balls
        base.Render(lidarData, origin);

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
                line.enabled = true;
                line.SetPosition(0, _ballCache[i].transform.position);
                line.SetPosition(1, origin.position);
            }
        }
    }
}
