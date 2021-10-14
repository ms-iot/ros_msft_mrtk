using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class visualizes the site that the simulated robot is currently operating within as seen by a lidar sensor.
/// </summary>
public class LidarVisualizer : MonoBehaviour
{
    /// <summary>
    /// Times per second that this GameObject should query the lidar and render the results.
    /// </summary>
    [SerializeField]
    [Tooltip("The number of times per second that the lidar is queried and the visualization is updated.")]
    [Range(1, 30)]
    public int renderCallsPerSecond = 1;
    public int lidarResolution = 360;
    public Vector2 randomRange = new Vector2(0.1f, 0.5f);
    public float ringHeight = 1f;

    /// <summary>
    /// An enum, used solely to acquire _provider using the factory class found in ILidarDataProvider.cs
    /// </summary>
    [SerializeField]
    [Tooltip("Specifies which data provider class to use in querying for lidar data.")]
    private LidarDataProviderClass lidarDataProviderType = LidarDataProviderClass.SIMPLE_RANDOM;
    /// <summary>
    /// The data provider class instance, used to query for lidar data.
    /// </summary>
    private ILidarDataProvider _provider;

    [SerializeField]
    [Tooltip("Laser Scan topic.")]
    public string topic = "/scan";

    /// <summary>
    /// An enum, used solely to acquire _renderer using the factory class found in ISpaceRenderer.cs
    /// </summary>
    [SerializeField]
    [Tooltip("Specifies which renderer class to use in visualizing the lidar data.")]
    private SpaceRendererClass spaceRendererType = SpaceRendererClass.BALL;

    /// <summary>
    /// Used to set the ball prefab
    /// </summary>
    [SerializeField]
    [Tooltip("Ball Visualization Prefab.")]
    public GameObject ballPrefab = null;

    /// <summary>
    /// Used to set the ball line material
    /// </summary>
    [SerializeField]
    [Tooltip("Material for Ball Line Visualization.")]
    public Material ballLineMaterial = null;

    /// <summary>
    /// The renderer class instance, used to render a given set of lidar data
    /// </summary>
    private ISpaceRenderer _renderer;



    // Start is called before the first frame update
    void Start()
    {
        _renderer = SpaceRenderer.GetSpaceRenderer(spaceRendererType, this);
        _provider = LidarDataProvider.GetLidarDataProvider(lidarDataProviderType, this);

        
        InvokeRepeating("RegenerateSite", 0f, 1f / renderCallsPerSecond);
    }

    /// <summary>
    /// Queries the lidar and renders the updated information
    /// </summary>
    public void RegenerateSite()
    {
        _renderer.Render(_provider.Query(), transform);
        
    }
}
