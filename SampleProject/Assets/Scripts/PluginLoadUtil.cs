using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using ROS2;

public static class PluginLoadUtil
{
    [DllImport("kernel32.dll", EntryPoint = "SetCurrentDirectoryA", SetLastError = true, ExactSpelling = true)]
    private static extern bool SetCurrentDirectoryA(string fileName);

    [DllImport("kernel32.dll", EntryPoint = "GetCurrentDirectoryA", SetLastError = true, ExactSpelling = true)]
    private static extern bool GetCurrentDirectoryA(uint nBufferLength, StringBuilder lpBuffer);

    private static string pluginPath = null;
    private static string defaultPath = null;


    public static void PerformPluginAction(Action func)
    {
        if (pluginPath == null)
        {
            GetPaths();
        }

        SetCurrentDirectoryA(pluginPath);

        func();

        SetCurrentDirectoryA(defaultPath);
    }

    public static void PerformPluginAction(Action<object[]> func, params object[] args)
    {
        if (pluginPath == null)
        {
            GetPaths();
        }

        SetCurrentDirectoryA(pluginPath);

        func(args);

        SetCurrentDirectoryA(defaultPath);
    }


    public static T PerformPluginAction<T>(Func<T> func)
    {
        if (pluginPath == null)
        {
            GetPaths();
        }

        SetCurrentDirectoryA(pluginPath);

        T output = func();

        SetCurrentDirectoryA(defaultPath);

        return output;
    }

    public static T PerformPluginAction<T>(Func<object[], T> func, params object[] args)
    {
        if (pluginPath == null)
        {
            GetPaths();
        }

        SetCurrentDirectoryA(pluginPath);

        T output = func(args);

        SetCurrentDirectoryA(defaultPath);

        return output;
    }

    private static void GetPaths()
    {
        var t = typeof(RclCppDotnet).Assembly.Location;
        Debug.Log("RCLdotnet location = " + t);
        pluginPath = Path.GetDirectoryName(t);

        var sb = new StringBuilder(256);
        GetCurrentDirectoryA((uint)sb.Capacity, sb);
        defaultPath = sb.ToString();
    }
}
