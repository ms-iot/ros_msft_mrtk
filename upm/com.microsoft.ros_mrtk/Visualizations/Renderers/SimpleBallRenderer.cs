using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using sensor_msgs.msg;

public class BallRenderer : ISpaceRenderer
{

    /// <summary>
    /// Prefab that is instantiated as the "ball"
    /// Used for this renderer's visualization.
    /// Currently Resource.Loaded from root of resources folder
    /// </summary>
    protected GameObject _ballPrefab;
    protected GameObject[] _ballCache;
    protected int _ballCacheSize;

    protected LidarVisualizer _owner;

    public BallRenderer()
    {
    }

    public virtual void Config(LidarVisualizer viz)
    {
        _owner = viz;
        _ballPrefab = _owner.ballPrefab;
    }

    public virtual void Render(LaserScan lidarData, Transform origin)
    {
        if (_ballPrefab == null)
        {
            return;
        }
        
        if (_ballCache == null)
        {
            _ballCacheSize = lidarData.Ranges.Count;
            _ballCache = new GameObject[_ballCacheSize];
        }

        ResizeCache(lidarData.Ranges.Count);

        for (int i = 0; i < _ballCacheSize; i++)
        {
            if (_ballCache[i] == null)
            {
                GameObject ball = GameObject.Instantiate(_ballPrefab, origin);
                _ballCache[i] = ball;
            }


            if ((lidarData.Ranges[i] > lidarData.Range_max) || (lidarData.Ranges[i] < lidarData.Range_min))
            {
                // Don't show data out of range
                _ballCache[i].SetActive(false);
            }
            else
            {
                float rad = lidarData.Angle_min + i * lidarData.Angle_increment; 
                Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * lidarData.Ranges[i];

                // wake up/activate the object if it wasn't used last frame
                _ballCache[i].SetActive(true);
                _ballCache[i].transform.localPosition = offset;
            }
        }
    }

    private void ResizeCache(int size)
    {
        if (size < _ballCacheSize)
        {
            // If the cache needs to be smaller, just update the size 
            // the slots of _ballCache after size are now logically 'garbage'
            // and should not be rendered
            for (int i = size; i < _ballCacheSize; i++)
            {
                _ballCache[i].SetActive(false);
            }
            _ballCacheSize = size;
            
        } 
        else if (size > _ballCacheSize)
        {
            // only rebuild the entire array if the new size exceeds the PHYSICAL size of the cache
            if (size > _ballCache.Length)
            {
                GameObject[] newCache = new GameObject[size];
                for (int i = 0; i < _ballCacheSize; i++)
                {
                    newCache[i] = _ballCache[i];
                }
                _ballCache = newCache;
            }
            
            _ballCacheSize = size;

            for (int i = 0; i < _ballCacheSize; i++)
            {
                _ballCache[i].SetActive(true);
            }
        }
    }
}
