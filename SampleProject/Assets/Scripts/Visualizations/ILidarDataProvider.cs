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
    SIMPLE_RANDOM,
    ROS
}

public static class LidarDataProvider
{
    public static ILidarDataProvider GetLidarDataProvider(LidarDataProviderClass ldpc, GameObject owner)
    {
        switch(ldpc)
        {
            case LidarDataProviderClass.SIMPLE_RANDOM:
                return new SimpleRandomDataProvider();
            case LidarDataProviderClass.ROS:
                return new ROS2LidarSubscription();
        }
        Debug.LogError("Unsupported lidar data provider was asked for");
        return null;
    }
}
