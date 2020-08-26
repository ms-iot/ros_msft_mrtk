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
    public bool DEBUG_DUMP_IMAGE;
    public string DEBUG_DUMP_IMAGE_NAME;
    

    private bool ready = false;
    private PhotoCapture captureObject = null;


    // Start is called before the first frame update
    private void Start()
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
    private void Update()
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
            if (DEBUG_DUMP_IMAGE)
            {
                Debug.LogWarning("Dumping webcam img...   " + string.Format(DEBUG_DUMP_IMAGE_NAME + "_{0}_webcam.jpg", System.DateTime.Now.Ticks));
                captureObject.TakePhotoAsync(string.Format(DEBUG_DUMP_IMAGE_NAME + "_{0}_webcam.jpg", System.DateTime.Now.Ticks), PhotoCaptureFileOutputFormat.JPG, OnPhotoCapturedToDisk);
            }
        } else
        {
            Debug.LogWarning("CapturePhoto called before webcam has successfully initialized!");
            PhotoCapture.PhotoCaptureResult failure = new PhotoCapture.PhotoCaptureResult();
            failure.resultType = PhotoCapture.CaptureResultType.UnknownError;
            callback(new PhotoCapture.PhotoCaptureResult(), null);
        }
    }

    private void OnPhotoCapturedToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("crackle!");
    }

    private void OnPhotoCaptureCreated(PhotoCapture capture)
    {
        captureObject = capture;
        // takes the highest resolution image supported
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Debug.Log(String.Format("Initializing camera with resolution: {0}x{1}", cameraResolution.width, cameraResolution.height));

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            ready = true;
        } else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
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
