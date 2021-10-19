using ROS2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sensor_msgs.msg;


/// <summary>
/// This implementation of ILidarDataProvider creates a configurable-resolutioned data reading of random floats between 8f and 10f.
/// </summary>
public class SimpleRandomDataProvider : ILidarDataProvider
{
    private LidarVisualizer _owner;
    // only allocate one array for this implementation
    protected List<float> _reserved;

    public SimpleRandomDataProvider() { }

    public void Config(LidarVisualizer viz)
    {
        _owner = viz;
        _reserved = new List<float>(_owner.lidarResolution);
        for (int i = 0; i < _owner.lidarResolution; i++)
        {
            _reserved.Add(_owner.randomRange.y * ((float)i / (float)_owner.lidarResolution) - _owner.randomRange.x);
        }
    }

    public LaserScan Query()
    {
        LaserScan scan = new LaserScan();
        scan.Angle_min = -Mathf.PI;
        scan.Angle_max = Mathf.PI;   // TODO: Make this configurable
        scan.Angle_increment = (float)(scan.Angle_max - scan.Angle_min) / _owner.lidarResolution;
        scan.Scan_time = _owner.renderCallsPerSecond;
        scan.Time_increment = _owner.renderCallsPerSecond;
        scan.Range_min = _owner.randomRange.x;
        scan.Range_max = _owner.randomRange.y;
        scan.Ranges = _reserved;

        if (!_owner.spiral)
        {
            // Replace with random data;
            for (int i = 0; i < _owner.lidarResolution; i++)
            {
                scan.Ranges[i] = Random.Range(_owner.randomRange.x, _owner.randomRange.y);
            }
        }

        return scan;
    }
}

