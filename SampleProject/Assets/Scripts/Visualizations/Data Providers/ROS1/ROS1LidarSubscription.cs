using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROS1LidarSubscription : MonoBehaviour
{
    private RosConnector _rc;

    // Update is called once per frame
    void Update()
    {
        if (_rc == null)
        {
            Init();
        }
    }

    private void Init()
    {
        
    }
}
