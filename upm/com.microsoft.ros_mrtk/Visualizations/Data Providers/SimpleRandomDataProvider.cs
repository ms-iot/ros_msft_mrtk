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
    }

    public LaserScan Query()
    {
        LaserScan scan = new LaserScan();
        scan.Angle_min = 0;
        scan.Angle_max = 360;
        scan.Angle_increment = 1;
        scan.Scan_time = _owner.renderCallsPerSecond;
        scan.Time_increment = _owner.renderCallsPerSecond;
        scan.Ranges = _reserved;

        for (int i = 0; i < _reserved.Capacity; i++)
        {
            _reserved[i] = Random.Range(_owner.randomRange.x, _owner.randomRange.y);
        }
        
        return scan;
    }
}

