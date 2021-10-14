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
        CleanData((flt) =>
        {
            return (flt > _curScan.Range_max) || (flt < _curScan.Range_min);
        });

        return _curScan;
    }
    /// <summary>
    /// Runs through the lidarData array/loop, replacing all points in the array classified as "dead" by the predicate
    /// with a linear interpolation value with the nearest non-dead values.
    /// </summary>
    /// <param name="deadPointClassify">A predicate function, which, given a float, returns if it is invalid (dead) data or not.</param>
    private void CleanData(Predicate<float> deadPointClassify)
    {
        //////////////////////////////////////////////////
        /// Edge Case - Loop edge element needs lerping
        {
            int startInd = _curScan.Ranges.Count - 1;
            int endInd = 0;
            int gap = 0;
            if (deadPointClassify(_curScan.Ranges[0]))
            {
                gap++;
                for (int i = 1; i < _curScan.Ranges.Count; i++)
                {
                    if (!deadPointClassify(_curScan.Ranges[i]))
                    {
                        gap += i - 1;
                        endInd = i;
                        break;
                    }
                }
                for (int i = 1; i < _curScan.Ranges.Count; i++)
                {
                    if (!deadPointClassify(_curScan.Ranges[_curScan.Ranges.Count - i]))
                    {
                        gap += i - 1;
                        startInd = _curScan.Ranges.Count - i;
                        break;
                    }
                }
            }
            else if (deadPointClassify(_curScan.Ranges[_curScan.Ranges.Count - 1]))
            {
                gap++;
                for (int i = 1; i < _curScan.Ranges.Count; i++)
                {
                    if (!deadPointClassify(_curScan.Ranges[_curScan.Ranges.Count - i]))
                    {
                        gap += i - 2;
                        startInd = _curScan.Ranges.Count - i;
                        break;
                    }
                }
            }
            if (gap > 0)
            {
                float ctr = 1f;
                for (int i = startInd + 1; i < startInd + gap + 1; i++)
                {
                    int boundedInd = i % _curScan.Ranges.Count;
                    float frac = ctr / (1f + (float)gap);
                    _curScan.Ranges[boundedInd] = Mathf.Lerp(_curScan.Ranges[startInd],
                        _curScan.Ranges[endInd], frac);
                    ctr++;
                }
            }
        }
        /// End Edge Case - Loop needs lerping
        //////////////////////////////////////////////////
        /// Base Case - Center portion of array
        {
            for (int i = 1; i < _curScan.Ranges.Count - 1; i++)
            {
                // If the point is dead, start looking for the nearest
                // non-dead points to the left and right of it.
                if (deadPointClassify(_curScan.Ranges[i]))
                {
                    int startInd = i - 1;
                    int endInd = i;
                    int gap = 1;
                    for (int j = 1; i + j < _curScan.Ranges.Count; j++)
                    {
                        if (!deadPointClassify(_curScan.Ranges[i + j]))
                        {
                            gap = j;
                            endInd = i + j;
                            break;
                        }
                    }
                    float ctr = 1f;
                    // Once the valid neighbor points are found,
                    // linearly interpolate the dead swathe of points between them.
                    for (int j = startInd + 1; j < endInd; j++)
                    {
                        float frac = ctr / (1f + (float)gap);
                        _curScan.Ranges[j] = Mathf.Lerp(_curScan.Ranges[startInd],
                            _curScan.Ranges[endInd], frac);
                        ctr++;
                    }
                }
            }
        }
        /// End Base Case
        //////////////////////////////////////////////////
    }
}
