using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This implementation of ILidarDataProvider creates a configurable-resolutioned data reading of random floats between 8f and 10f.
/// </summary>
public class SimpleRandomDataProvider : ILidarDataProvider
{
    private LidarVisualizer _owner;
    // only allocate one array for this implementation
    protected float[] _reserved;

    public SimpleRandomDataProvider() { }

    public void Config(LidarVisualizer viz)
    {
        _owner = viz;
        _reserved = new float[_owner.lidarResolution];
    }

    public float[] Query()
    {
        for (int i = 0; i < _reserved.Length; i++)
        {
            _reserved[i] = Random.Range(_owner.randomRange.x, _owner.randomRange.y);
        }
        return _reserved;
    }
}

