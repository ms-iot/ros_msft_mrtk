using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


#region Apriltag Structures

[StructLayout(LayoutKind.Sequential)]
public struct ZArray
{
    public ulong el_sz;
    public int size;
    public int alloc;
    public IntPtr data;
}


[StructLayout(LayoutKind.Sequential)]
public unsafe struct ApriltagDetection
{
    public IntPtr family;
    public int id;
    public int hamming;
    public float decision_margin;

    public Matd* H;

    public fixed double c[2];
    public fixed double p[8];
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Matd
{
    public uint nrows;
    public uint ncols;
    public double* data;
}

[StructLayout(LayoutKind.Sequential)]
public struct AprilTagDetectionInfo
{
    public IntPtr det;
    public double tagsize;
    public double fx;
    public double fy;
    public double cx;
    public double cy;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AprilTagPose
{
    public Matd* R;
    public Matd* t;
}

[StructLayout(LayoutKind.Sequential)]
public struct image_u8
{
    public int width;
    public int height;
    public int stride;

    public IntPtr buf;
}

#endregion // Apriltag Structures


#region Calibration Structures

[StructLayout(LayoutKind.Sequential)]
public struct Intrensics
{
    public double fx;
    public double fy;
    public double cx;
    public double cy;
}

#endregion  // Calibration Structures
