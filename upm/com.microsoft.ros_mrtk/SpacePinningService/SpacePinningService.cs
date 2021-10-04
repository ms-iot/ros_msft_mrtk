using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.WebCam;
using UnityEngine.XR.WSA;
using Microsoft.MixedReality.QR;

public class SpacePinningService : MonoBehaviour
{
    protected static SpacePinningService _instance;

    //private TransformListener _listener;
    //private WorldAnchor _pinning;

    private struct RelTransform
    {
        public Vector3 translation;
        public Quaternion rotation;

        public RelTransform(Vector3 vec, Quaternion rot)
        {
            translation = vec;
            rotation = rot;
        }
        
    }

    public static SpacePinningService instance 
    { 
        get 
        {
            _instance = _instance ?? Init();
            return _instance;
        }
    }


    private static SpacePinningService Init()
    {
        var instance = FindObjectOfType<SpacePinningService>();
        if (instance != null)
        {
            Debug.LogWarning("SpacePinningService is already in the scene");
            return instance;
        }

        GameObject obj = new GameObject("SpacePinningService");
        instance = obj.AddComponent<SpacePinningService>();

        DontDestroyOnLoad(instance);

        //_listener = new TransformListener();

        return instance;
    }

/*
    /// <summary>
    /// Attempts to locate the ROS world origin, placing a world anchor at that location if found.
    /// </summary>
    /// <returns>
    /// true if a world anchor is created successfully, otherwise false.
    /// A world anchor could fail to be created for a number of reasons: webcam image did not have
    /// an identifiable apriltag, tf2 messages are not being found (no robot discovered, incorrect frame name)
    /// etc.
    /// </returns>
    private bool UpdateSpacePinning(WebcamSystem.CaptureFrameInstance captureFrame)
    {   

        if (_pinning != null)
        {
            return false;
        }

        // Use the fiducial tag labeled '1' to pin the anchor
        if (fiducialCentersRelWebcam.ContainsKey(1))
        {
            // Raw TF values follow a different xyz coordinate system, must be converted with the TransformHelper class
            TfVector3? worldZeroRelFiducialCenterTranslationTF = _listener.LookupTranslation("fiducial_link", "odom");
            TfQuaternion? worldZeroRelFiducialCenterRotationTF = _listener.LookupRotation("fiducial_link", "odom");

            if (worldZeroRelFiducialCenterTranslationTF.HasValue && worldZeroRelFiducialCenterRotationTF.HasValue)
            {
                 Vector3 worldZeroRelFiducialCenterTranslation = TransformHelper.VectorTfToUnity(worldZeroRelFiducialCenterTranslationTF.Value);
                Quaternion worldZeroRelFiducialCenterRotation = TransformHelper.QuatTfToUnity(worldZeroRelFiducialCenterRotationTF.Value);
                // translate from camera pos -> fiducial tag pos
                Vector3 fiducialPos = Camera.main.transform.position + fiducialCentersRelWebcam[1].translation;
                // rotate from camera space -> fiducial space
                Quaternion fiducialRot = Camera.main.transform.rotation * fiducialCentersRelWebcam[1].rotation; 

                GameObject anchor = new GameObject("WorldZero");
                _pinning = anchor.AddComponent<WorldAnchor>();
                // first put anchor at fiducial location...
                anchor.transform.position = fiducialPos;
                anchor.transform.rotation = fiducialRot;
                Collocator.StartCollocation(_pinning);
                Debug.Log("Anchor laid at Unity-space coords" + anchor.transform.position);
                return true;
            } else
            {
                Debug.LogError("TF2 failed to query between fiducial tag and ROS world zero... is the ROS graph active?");
            }
        }
        
        return false; 
    }
*/
}
