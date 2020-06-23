using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime;
using System.Runtime.InteropServices;

using ROS2;
using ROS2.Common;
public class ROS2Listener : MonoBehaviour
{
      [DllImport ("kernel32.dll", EntryPoint = "SetCurrentDirectoryA", SetLastError = true, ExactSpelling = true)]
      private static extern bool SetCurrentDirectoryA (string fileName);

      [DllImport ("kernel32.dll", EntryPoint = "GetCurrentDirectoryA", SetLastError = true, ExactSpelling = true)]
      private static extern bool GetCurrentDirectoryA(uint nBufferLength, StringBuilder lpBuffer);

    public INode node = null;
    private ISubscription<std_msgs.msg.String> chatter_sub;
    private Dictionary<String, ISubscriptionBase> _singletonSubs;


    private static ROS2Listener _instance;

    public static ROS2Listener instance { get {
            _instance = _instance ?? Init();
            return _instance;
        } }

    /// <summary>
    /// Called only by the instance singleton getter when the instance has not yet been initialized.
    /// </summary>
    /// <returns>The instance of this singleton class</returns>
    private static ROS2Listener Init()
    {
        // attempt to find an instance already in the scene
        _instance = FindObjectOfType<ROS2Listener>();
        if (_instance != null)
        {
            Debug.LogWarning("ROS2Listener.Init() is being called even when the singleton instance already exists!");
            return _instance;
        }

        Debug.Log("ROS is Awake");

        GameObject obj = new GameObject("ROS2Listener");
        _instance = obj.AddComponent<ROS2Listener>();

        var t = typeof(RCLdotnet).Assembly.Location;
        Debug.Log("RCLdotnet location = " + t);
        var p = Path.GetDirectoryName(t);
        var sb = new StringBuilder(256);
        GetCurrentDirectoryA((uint)sb.Capacity, sb);
        try
        {
            SetCurrentDirectoryA(p);

            RCLRet ret = RCLdotnet.Init();
            if (ret == RCLRet.Ok)
            {
                Debug.Log("ROS is using " + RCLdotnet.GetRMWIdentifier());
            }
            else
            {
                Debug.Log("RCL InitE = " + RCLdotnet.GetErrorString());
            }

            _instance.node = RCLdotnet.CreateNode("listener");

            _instance._singletonSubs = new Dictionary<string, ISubscriptionBase>();
        }
        catch (Exception e)
        {
            Destroy(_instance.gameObject);
            _instance = null;
            Debug.Log(e.ToString());
        }
        SetCurrentDirectoryA(sb.ToString());
        DontDestroyOnLoad(_instance);
        return _instance;
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
