using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROS1LidarSubscription : LaserScanVisualizer, ILidarDataProvider
{
    private RosConnector _rc;
    private LaserScanSubscriber _sub;

    private LidarVisualizer _owner;

    public void Init()
    {
        _rc = gameObject.AddComponent<RosConnector>();
        _rc.Timeout = _owner.rosConnectorTimeout;
        _rc.RosBridgeServerUrl = _owner.rosBridgeURL;

        Invoke("InitDelayed", 0.1f);
    }

    private void InitDelayed()
    {
        _sub = gameObject.AddComponent<LaserScanSubscriber>();
        _sub.Topic = _owner.topic;
        _sub.TimeStep = 0;

        Invoke("InitFinal", 0.1f);
    }

    private void InitFinal()
    {
        _sub.laserScanWriter = gameObject.AddComponent<LaserScanWriter>();
    }

    public float[] Query()
    {
        return this.ranges ?? new float[_owner.lidarResolution];
    }

    public void Config(LidarVisualizer viz)
    {
        _owner = viz;
    }

    protected override void Visualize() { }


    protected override void DestroyObjects() { }
}
