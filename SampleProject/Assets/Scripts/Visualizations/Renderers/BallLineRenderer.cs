using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLineRenderer : BallRenderer
{
    public BallLineRenderer() : base()
    {
        
    }

    public override void Render(float[] lidarData, Transform origin)
    {
        base.Render(lidarData, origin);
        for (int i = 0; i < _ballCacheSize; i++)
        {
            Debug.DrawLine(origin.position, _ballCache[i].transform.position, Color.blue, 1f);
        }
    }
}
