using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This implementation of ILidarDataProvider creates a 1000-resolutioned data reading of random floats between 8f and 10f.
/// </summary>
public class SimpleRandomDataProvider : ILidarDataProvider
{
    protected float[] _reserved;  // only allocate one array for this implementation

    public SimpleRandomDataProvider()
    {
        _reserved = new float[1000];
    }


    public float[] Query()
    {
        for (int i = 0; i < _reserved.Length; i++)
        {
            _reserved[i] = Random.Range(8f, 10f);
        }
        return _reserved;  // TODO is it safe to expose _reserved?
    }
}
