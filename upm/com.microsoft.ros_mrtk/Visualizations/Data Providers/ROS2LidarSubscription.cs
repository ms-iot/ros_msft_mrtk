using ROS2;
using sensor_msgs.msg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class ROS2LidarSubscription : ILidarDataProvider
{
    private LidarVisualizer _owner;

    private ISubscription<LaserScan> _sub;
    private ROS2Listener _r2l;
    private LaserScan _curScan;

    public ROS2LidarSubscription()
    {
    }

    public void Config(LidarVisualizer viz)
    {
        _owner = viz;

        _r2l = ROS2Listener.instance;
        _sub = _r2l.node.CreateSubscription<LaserScan>(
            _owner.topic, msg => {
                _curScan = msg;
            }, ROS2.Utils.QosProfile.Profile.SensorData);
    }

    public LaserScan Query()
    {
        return _curScan;
    }
}
