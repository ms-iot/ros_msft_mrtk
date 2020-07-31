using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;


/// <summary>
/// This class visualizes the site that the simulated robot is currently operating within as seen by a lidar sensor.
/// </summary>
public class LidarVisualizer : MonoBehaviour
{
    #region config
    #region globalFields
    /// <summary>
    /// Times per second that this GameObject should query the lidar and render the results.
    /// </summary>
    [SerializeField]
    [Tooltip("The number of times per second that the lidar is queried and the visualization is updated.")]
    [Range(1, 30)]
    private int renderCallsPerSecond;

    public int lidarResolution = 360;
    public float worldScale = 1f;
    #endregion // globalFields
    #region ILidarDataProviderConfig
    #region simpleRandom 
    public Vector2 randomRange;
    #endregion // simpleRandom
    #region ros1
    public int rosConnectorTimeout = 10;
    public string rosBridgeURL = "ws://127.0.0.1:9090";
    public string topic = "/scan";
    #endregion // ros1
    #endregion //ILidarDataProviderConfig
    #region ISpaceRendererConfig
    #region ringMeshFields
    public float ringHeight = 1f;
    #endregion // ringMeshFields
    #endregion // ISpaceRendererConfig
    #endregion // config

    

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
    private void RegenerateSite()
    {
        float[] data = _provider.Query();
        _renderer.Render(data, transform);
        
    }
}
