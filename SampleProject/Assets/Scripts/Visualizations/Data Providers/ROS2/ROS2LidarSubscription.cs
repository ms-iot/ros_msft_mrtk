using ROS2;
using sensor_msgs.msg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROS2LidarSubscription : ILidarDataProvider
{
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

    public float[] Query()
    {
        if (_curScan != null)
        {
            return _curScan.Ranges.ToArray();
        } else
        {
            return new float[360];
        }
    }
}



