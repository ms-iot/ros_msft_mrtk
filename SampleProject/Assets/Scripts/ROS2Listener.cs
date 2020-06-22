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

    private INode node = null; 
    private ISubscription<std_msgs.msg.String> chatter_sub;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ROS is Awake");

        var t = typeof(RCLdotnet).Assembly.Location;
        Debug.Log("RCLdotnet location = " + t);
        var p = Path.GetDirectoryName(t);
        var sb = new StringBuilder(256);
        GetCurrentDirectoryA((uint)sb.Capacity, sb);
        try
        {
            SetCurrentDirectoryA(p);

            RCLRet ret = RCLdotnet.Init ();
            if (ret == RCLRet.Ok)
            {
                Debug.Log("ROS is using " + RCLdotnet.GetRMWIdentifier());
            }
            else
            {
                Debug.Log("RCL InitE = " + RCLdotnet.GetErrorString());
            }

            node = RCLdotnet.CreateNode ("listener");

            chatter_sub = node.CreateSubscription<std_msgs.msg.String> (
                "chatter", msg => Debug.Log("I heard: [" + msg.Data + "]"));        

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        SetCurrentDirectoryA(sb.ToString());
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
