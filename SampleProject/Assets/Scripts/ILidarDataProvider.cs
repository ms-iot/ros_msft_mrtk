using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILidarDataProvider
{
    // probably will become async in future, not sure what the ROS framework entails
    float[] Query();  
}

public enum LidarDataProviderClass
{
    SIMPLE_RANDOM
}

public static class LidarDataProvider
{
    public static ILidarDataProvider GetLidarDataProvider(LidarDataProviderClass ldpc)
    {
        switch(ldpc)
        {
            case LidarDataProviderClass.SIMPLE_RANDOM:
                return new SimpleRandomDataProvider();  
        }
        Debug.LogError("Unsupported lidar data provider was asked for");
        return null;
    }
}
