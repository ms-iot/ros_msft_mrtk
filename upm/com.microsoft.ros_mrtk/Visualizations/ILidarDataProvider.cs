using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sensor_msgs.msg;

public interface ILidarDataProvider
{
    // probably will become async in future, not sure what the ROS framework entails
    LaserScan Query();
    void Config(LidarVisualizer viz);
}


public enum LidarDataProviderClass
{
    SIMPLE_RANDOM,
    ROS2
}

public static class LidarDataProvider
{
    public static ILidarDataProvider GetLidarDataProvider(LidarDataProviderClass ldpc, LidarVisualizer owner)
    {
        ILidarDataProvider provider = null;
        switch (ldpc)
        {
            case LidarDataProviderClass.SIMPLE_RANDOM:
                provider = new SimpleRandomDataProvider();
                break;
            case LidarDataProviderClass.ROS2:
                provider = new ROS2LidarSubscription();
                break;
        }
        if (provider == null)
        {
            Debug.LogError("Unsupported lidar data provider was asked for");
            return null;
        }
        provider.Config(owner);
        return provider;
    }
}
