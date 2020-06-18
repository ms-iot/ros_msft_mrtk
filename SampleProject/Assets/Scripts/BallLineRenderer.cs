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
        foreach(GameObject sphere in _ballCache)
        {
            Debug.DrawLine(origin.position, sphere.transform.position, Color.blue);
        }
    }
}
