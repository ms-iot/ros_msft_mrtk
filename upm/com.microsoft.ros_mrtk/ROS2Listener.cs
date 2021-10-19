using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ROS2;
using ROS2.Common;

public class ROS2Listener : MonoBehaviour
{
    public INode node = null;

    private static ROS2Listener _instance = null;

    public static ROS2Listener instance 
    { 
        get 
        {
            _instance = _instance ?? Init();
            return _instance;
        }
    }

    /// <summary>
    /// Called only by the instance singleton getter when the instance has not yet been initialized.
    /// </summary>
    /// <returns>The instance of this singleton class</returns>
    private static ROS2Listener Init()
    {
        // attempt to find an instance already in the scene
        var instance = FindObjectOfType<ROS2Listener>();
        if (instance != null)
        {
            Debug.LogWarning("ROS2 Listender is already in the scene");
            return instance;
        }

        Debug.Log("ROS is Awake");

        GameObject obj = new GameObject("ROS2Listener");
        instance = obj.AddComponent<ROS2Listener>();
        try
        {
            RCLRet ret = RCLdotnet.Init();
            if (ret == RCLRet.Ok)
            {
                Debug.Log("ROS is using " + RCLdotnet.GetRMWIdentifier());
            }
            else
            {
                Debug.Log("RCL Init Error = " + RCLdotnet.GetErrorString());
            }

            instance.node = RCLdotnet.CreateNode("listener");

        }
        catch (Exception e)
        {
            Destroy(instance.gameObject);
            instance = null;
            Debug.Log(e.ToString());
        }
        DontDestroyOnLoad(instance);
        return instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (node != null)
        {
            RCLdotnet.SpinOnce(node, 0);
        }
    }
}
