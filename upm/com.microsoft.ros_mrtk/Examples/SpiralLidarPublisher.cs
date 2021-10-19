using ROS2;
using ROS2.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sensor_msgs.msg;


public class SpiralLidar : MonoBehaviour
{
    IPublisher<LaserScan> scanPub;
    LaserScan scan = new LaserScan();

    void Start()
    {
        scanPub = ROS2Listener.instance.node.CreatePublisher<LaserScan> ("scanExample", QosProfile.Profile.SensorData);

        scan.Header.Frame_id = "scanExample";
        scan.Angle_min = -Mathf.PI;
        scan.Angle_max = Mathf.PI;
        scan.Angle_increment = 2.0f * Mathf.PI / 360;
        scan.Scan_time = 1;
        scan.Time_increment = 1;
        scan.Range_min = 0;
        scan.Range_max = 1;
        for (int i = 0; i < 360; i++)
        {
            scan.Ranges.Add(1 * ((float)i / (float)360));
        }

        InvokeRepeating("pub", 0f, 1f);
    }

    public void pub()
    {
        scanPub.Publish (scan);
    }


}