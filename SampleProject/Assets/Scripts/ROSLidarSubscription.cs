using ROS2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROSLidarSubscription : ILidarDataProvider
{
    private ISubscription<sensor_msgs.msg.LaserScan> _sub;
    private ROS2Listener _r2l;
    private sensor_msgs.msg.LaserScan _curScan;

    

    public ROSLidarSubscription()
    {
        _r2l = ROS2Listener.instance;
        _sub = _r2l.node.CreateSubscription<sensor_msgs.msg.LaserScan>(
            "scan", msg => {
                _curScan = msg;
                Debug.Log("Scan message: " + msg);
            });        
    }

    public float[] Query()
    {
        return null; 
    }
}



