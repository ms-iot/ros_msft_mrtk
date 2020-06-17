using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRandomDataProvider : ILidarDataProvider
{
    public void Query(float[] dst)
    {

        for (int i = 0; i < dst.Length; i++)
        {
            dst[i] = Random.Range(8f, 10f);
        }
        
    }
}
