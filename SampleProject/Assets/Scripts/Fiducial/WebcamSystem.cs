using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Windows.WebCam;

public partial class WebcamSystem : MonoBehaviour
{
    public static WebcamSystem instance;

    public ComputeShader BGRAtoGrayscaleShader;
    public Resolution cameraResolution;

    private bool ready = false;
    private PhotoCapture captureObject = null;
    

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        } else
        {
            Debug.LogWarning("Duplicate WebcamSystem tried to initialize in scene on gameobject " + this.gameObject + "; Destroying self!");
            Destroy(this);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == false)
        {
            this.Shutdown();
        }
#endif
    }

    private void OnApplicationQuit()
    {
        this.Shutdown();
    }

    public void CapturePhoto(PhotoCapture.OnCapturedToMemoryCallback callback)
    {
        if (ready)
        {
            captureObject.TakePhotoAsync(callback);
        } else
        {
            Debug.LogWarning("CapturePhoto called before webcam has successfully initialized!");
        }
    }

    void OnPhotoCaptureCreated(PhotoCapture capture)
    {
        captureObject = capture;
        // takes the highest resolution image supported
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Debug.Log(string.Format("Initializing camera with resolution: {0}x{1}", cameraResolution.width, cameraResolution.height));

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            ready = true;
        } else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        captureObject.Dispose();
        captureObject = null;
        ready = false;
    }

    private void Shutdown()
    {
        if (captureObject != null && ready)
        {
            ready = false;
            captureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            CaptureFrameInstance.DisposeBuffers();
        }
    }
}
