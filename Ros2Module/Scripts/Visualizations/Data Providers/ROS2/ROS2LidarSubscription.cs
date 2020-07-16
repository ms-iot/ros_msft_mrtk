using ROS2;
using sensor_msgs.msg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROS2LidarSubscription : ILidarDataProvider
{
    private LidarVisualizer _owner;

    private ISubscription<LaserScan> _sub;
    private ROS2Listener _r2l;
    private LaserScan _curScan;

    public ROS2LidarSubscription()
    {
        _r2l = ROS2Listener.instance;
        _sub = _r2l.node.CreateSubscription<LaserScan>(
            "scan", msg => {
                _curScan = msg;
            }, ROS2.Utils.QosProfile.Profile.SensorData);        
    }

    public void Config(LidarVisualizer viz)
    {
        _owner = viz;
    }

    public float[] Query()
    {
        if (_curScan != null)
        {
            var ranges = _curScan.Ranges.ToArray();
            if (ranges.Length != _owner.lidarResolution)
            {
                Debug.LogError(@"ROS2 subscription is reading lidar
                    data of different resolution than configured!
                    expected: " + _owner.lidarResolution + ", recieved: " + ranges.Length);
                return new float[_owner.lidarResolution];
            }
            return ranges;
        } else
        {
            return new float[_owner.lidarResolution];
        }
    }
}



