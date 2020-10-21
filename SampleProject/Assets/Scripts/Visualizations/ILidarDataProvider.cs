using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILidarDataProvider
{
    // probably will become async in future, not sure what the ROS framework entails
    float[] Query();
    void Config(LidarVisualizer viz);
}


public enum LidarDataProviderClass
{
    SIMPLE_RANDOM,
    ROS1,
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
            case LidarDataProviderClass.ROS1:
#if ROS1_MODULE_LIDAR
                ROS1LidarSubscription sub = owner.gameObject.AddComponent<ROS1LidarSubscription>();
                sub.Config(owner);
                sub.Init();
                provider = sub; 
#else
                Debug.LogError("ROS1 data provider classes not imported!");
#endif
                break; 
            case LidarDataProviderClass.ROS2:
#if ROS2_MODULE_LIDAR
                provider = new ROS2LidarSubscription();
#else
                Debug.LogError("ROS2 data provider classes not imported!");
#endif
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
