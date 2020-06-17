using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;


/// <summary>
/// This class visualizes the site that the simulated robot is currently operating within.
/// </summary>
public class OperationSite : MonoBehaviour
{
    /// <summary>
    /// Times per second that this GameObject should query the lidar and render the results.
    /// </summary>
    [SerializeField]
    [Tooltip("The number of times per second that the lidar is queried and the visualization is updated.")]
    [Range(1, 30)]
    private int renderCallsPerSecond;

    /// <summary>
    /// The number of samples the lidar sensor takes.
    /// </summary>
    [SerializeField]
    [Tooltip("Number of samples taken by the lidar.")]
    private int lidarResolution = 100;

    /// <summary>
    /// An enum, used solely to acquire _provider using the factory class found in ILidarDataProvider.cs
    /// </summary>
    [SerializeField]
    [Tooltip("Specifies which data provider class (implements ILidarDataProvider) to use in querying for lidar data.")]
    private LidarDataProviderClass lidarDataProviderType = LidarDataProviderClass.SIMPLE_RANDOM;
    /// <summary>
    /// The data provider class instance, used to query for lidar data.
    /// </summary>
    private ILidarDataProvider _provider;

    /// <summary>
    /// An enum, used solely to acquire _renderer using the factory class found in ISpaceRenderer.cs
    /// </summary>
    [SerializeField]
    [Tooltip("Specifies which renderer class (implements ISpaceRenderer) to use in visualizing the lidar data.")]
    private SpaceRendererClass spaceRendererType = SpaceRendererClass.BALL;
    /// <summary>
    /// The renderer class instance, used to render a given set of lidar data
    /// </summary>
    private ISpaceRenderer _renderer;


    

    
    
    private float[] _lidarData;  // the format for lidardata may change depending on what I am given by ROS.net


    // Start is called before the first frame update
    void Start()
    {
        _renderer = SpaceRenderer.GetSpaceRenderer(spaceRendererType);
        _provider = LidarDataProvider.GetLidarDataProvider(lidarDataProviderType);

        _lidarData = new float[lidarResolution];
        
        InvokeRepeating("RegenerateSite", 0f, 1f / renderCallsPerSecond);
        //Invoke("RegenerateSite", 0f);
    }

    /// <summary>
    /// Queries the lidar and renders the updated information
    /// </summary>
    private void RegenerateSite()
    {
        Debug.Log("Regenerating");
        _provider.Query(_lidarData);
        _renderer.Render(_lidarData, transform);
    }
}
