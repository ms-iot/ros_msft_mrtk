using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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
                ROS1LidarSubscription sub = owner.gameObject.AddComponent<ROS1LidarSubscription>();
                sub.Config(owner);
                sub.Init();
                provider = sub;
                break;
            case LidarDataProviderClass.ROS2:
                provider = new ROS2LidarSubscription();
                break;
        }
        if (provider == null)
        {
            Debug.LogError("Unsupported lidar data provider was asked for");
        }
        provider.Config(owner);
        return provider;
    }
}
