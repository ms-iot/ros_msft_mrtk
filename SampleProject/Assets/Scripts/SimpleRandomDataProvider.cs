using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This implementation of ILidarDataProvider creates a 1000-resolutioned data reading of random floats between 8f and 10f.
/// </summary>
public class SimpleRandomDataProvider : ILidarDataProvider
{
    // only allocate one array for this implementation
    protected float[] _reserved;  

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
        return _reserved;
    }
}
