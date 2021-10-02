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
        _r2l = ROS2Listener.instance;
        _sub = _r2l.node.CreateSubscription<LaserScan>(
            "scan", msg => {                                // TODO: Scan should be configurable
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
            CleanData(ref ranges);
            return ranges;
        }
        else
        {
            return new float[_owner.lidarResolution];
        }
    }

    private void CleanData(ref float[] lidarData)
    {
        LinearlyInterpolateDeadPoints(ref lidarData, (flt) =>
        {
            return float.IsInfinity(flt) || flt < 0.01f;
        });
    }

    /// <summary>
    /// Runs through the lidarData array/loop, replacing all points in the array classified as "dead" by the predicate
    /// with a linear interpolation value with the nearest non-dead values.
    /// </summary>
    /// <param name="lidarData">A reference to an array, representing the lidar data ring (0th el is next to last el).</param>
    /// <param name="deadPointClassify">A predicate function, which, given a float, returns if it is invalid (dead) data or not.</param>
    private void LinearlyInterpolateDeadPoints(ref float[] lidarData, Predicate<float> deadPointClassify)
    {
        //////////////////////////////////////////////////
        /// Edge Case - Loop edge element needs lerping
        {
            int startInd = lidarData.Length - 1;
            int endInd = 0;
            int gap = 0;
            if (deadPointClassify(lidarData[0]))
            {
                gap++;
                for (int i = 1; i < lidarData.Length; i++)
                {
                    if (!deadPointClassify(lidarData[i]))
                    {
                        gap += i - 1;
                        endInd = i;
                        break;
                    }
                }
                for (int i = 1; i < lidarData.Length; i++)
                {
                    if (!deadPointClassify(lidarData[lidarData.Length - i]))
                    {
                        gap += i - 1;
                        startInd = lidarData.Length - i;
                        break;
                    }
                }
            }
            else if (deadPointClassify(lidarData[lidarData.Length - 1]))
            {
                gap++;
                for (int i = 1; i < lidarData.Length; i++)
                {
                    if (!deadPointClassify(lidarData[lidarData.Length - i]))
                    {
                        gap += i - 2;
                        startInd = lidarData.Length - i;
                        break;
                    }
                }
            }
            if (gap > 0)
            {
                float ctr = 1f;
                for (int i = startInd + 1; i < startInd + gap + 1; i++)
                {
                    int boundedInd = i % lidarData.Length;
                    float frac = ctr / (1f + (float)gap);
                    lidarData[boundedInd] = Mathf.Lerp(lidarData[startInd],
                        lidarData[endInd], frac);
                    ctr++;
                }
            }
        }
        /// End Edge Case - Loop needs lerping
        //////////////////////////////////////////////////
        /// Base Case - Center portion of array
        {
            for (int i = 1; i < lidarData.Length - 1; i++)
            {
                // If the point is dead, start looking for the nearest
                // non-dead points to the left and right of it.
                if (deadPointClassify(lidarData[i]))
                {
                    int startInd = i - 1;
                    int endInd = i;
                    int gap = 1;
                    for (int j = 1; i + j < lidarData.Length; j++)
                    {
                        if (!deadPointClassify(lidarData[i + j]))
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
                        lidarData[j] = Mathf.Lerp(lidarData[startInd],
                            lidarData[endInd], frac);
                        ctr++;
                    }
                }
            }
        }
        /// End Base Case
        //////////////////////////////////////////////////
    }
}
