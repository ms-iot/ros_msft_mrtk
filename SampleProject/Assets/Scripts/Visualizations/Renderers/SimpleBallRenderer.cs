using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public BallRenderer()
    {
        _ballPrefab = Resources.Load<GameObject>("Sphere");

        if (_ballPrefab == null)
        {
            Debug.LogError("BallRenderer failed to locate the ball prefab!");
        }
    }

    
    public virtual void Render(float[] lidarData, Transform origin)
    {
        if (_ballCache == null)
        {
            _ballCache = new GameObject[lidarData.Length];
            _ballCacheSize = lidarData.Length;
        }

        ResizeCache(lidarData.Length);

        for (int i = 0; i < _ballCacheSize; i++)
        {
            if (_ballCache[i] == null)
            {
                GameObject ball = GameObject.Instantiate(_ballPrefab, origin);
                _ballCache[i] = ball;
            }
            float rad = ((float)i / (float)lidarData.Length) * (2 * Mathf.PI);
            // offset by 90 degrees so that first data point corresponds to x axis/straight ahead
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * lidarData[i];  
            // wake up/activate the object if it wasn't used last frame
            _ballCache[i].SetActive(true);
            _ballCache[i].transform.localPosition = offset;
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
            
        } else if (size > _ballCacheSize)
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
        }
    }
}
