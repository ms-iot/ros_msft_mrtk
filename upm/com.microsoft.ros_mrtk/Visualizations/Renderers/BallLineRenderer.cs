using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLineRenderer : BallRenderer
{
    private LineRenderer[] _lineCache;
    private int _lineCacheSize;

    public BallLineRenderer() : base()
    {

    }

    public override void Render(float[] lidarData, Transform origin)
    {
        // Move the balls
        base.Render(lidarData, origin);

        if (_lineCache == null)
        {
            _lineCache = new LineRenderer[lidarData.Length];
            _lineCacheSize = lidarData.Length;
        }

        ResizeCache(lidarData.Length);

        for (int i = 0; i < _lineCacheSize; i++)
        {
            if (_lineCache[i] == null)
            {
                LineRenderer line = _ballCache[i].GetComponent<LineRenderer>();
                line.startColor = Color.blue;
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                _lineCache[i] = line;
            }
            
            // wake up/activate the object if it wasn't used last frame
            _lineCache[i].enabled = true;
            _lineCache[i].SetPosition(0, _ballCache[i].transform.position);
            _lineCache[i].SetPosition(1, origin.position);
        }

    }

    private void ResizeCache(int size)
    {
        if (size < _lineCacheSize)
        {
            // If the cache needs to be smaller, just update the size 
            // the slots of _ballCache after size are now logically 'garbage'
            // and should not be rendered
            for (int i = size; i < _lineCacheSize; i++)
            {
                _lineCache[i].enabled = false;
            }
            _lineCacheSize = size;

        }
        else if (size > _lineCacheSize)
        {
            // only rebuild the entire array if the new size exceeds the PHYSICAL size of the cache
            if (size > _lineCache.Length)
            {
                LineRenderer[] newCache = new LineRenderer[size];
                for (int i = 0; i < _lineCacheSize; i++)
                {
                    newCache[i] = _lineCache[i];
                }
                _lineCache = newCache;
            }

            _lineCacheSize = size;

            for (int i = 0; i < _lineCacheSize; i++)
            {
                _lineCache[i].enabled = true;
            }
        }
    }
}
