using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Windows.WebCam;

public class WebcamSystem : MonoBehaviour
{
    private static WebcamSystem instance;

    private bool ready = false;
    private PhotoCapture captureObject = null;
    private Resolution cameraResolution;
    private CaptureFrameInstance currFrame = null;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        } else
        {
            Debug.LogWarning("Duplicate WebcamSystem tried to initialize in scene on gameobject " + this.gameObject + "; Destroying self!");
            Destroy(this);
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
            captureObject.TakePhotoAsync(OnCapturedPhotoToMemory); // DEBUG
        } else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame)
    {
        if (result.success)
        {
            Debug.Log("SNAP!!!");
            currFrame = new CaptureFrameInstance(frame, cameraResolution);
        } else
        {
            Debug.LogError("Unable to take photo!");
        }
        
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        captureObject.Dispose();
        captureObject = null;
        ready = false;
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

    private void Shutdown()
    {
        if (captureObject != null && ready)
        {
            ready = false;
            captureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
    }

    /// <summary>
    /// As an optimization, we skip copying the image data (buf) by pointing to the
    /// buffer given to the managed PhotoCaptureFrame. Since the PhotoCaptureFrame is managed,
    /// we must avoid premature garbage collection of the managed buffer by 
    /// stashing a reference in the object
    /// </summary>
    public class CaptureFrameInstance
    {
        public PhotoCaptureFrame managedFrame;
        public System.IntPtr unmanagedFrame;

        private System.IntPtr bufPtr;

        public CaptureFrameInstance(PhotoCaptureFrame managedFrame, Resolution res)
        {
            this.managedFrame = managedFrame;

            FiducialSystem.image_u8 temp = new FiducialSystem.image_u8();
            temp.width = res.width;
            temp.height = res.height;
            temp.stride = res.width;
            temp.buf = Marshal.AllocHGlobal(temp.height * temp.stride);

            List<byte> managedBuffer = new List<byte>();
            managedFrame.CopyRawImageDataIntoBuffer(managedBuffer);
            byte[] transformedImg = new byte[temp.width * temp.height];
            ProcessImage(managedBuffer.ToArray(), temp.width, temp.height, transformedImg);

            Marshal.Copy(transformedImg, 0, temp.buf, transformedImg.Length);
            unmanagedFrame = Marshal.AllocHGlobal(Marshal.SizeOf(temp));
            Marshal.StructureToPtr<FiducialSystem.image_u8>(temp, unmanagedFrame, false);


            this.bufPtr = temp.buf;
            FiducialSystem.instance.UpdateSpacePinning(this);
        }

        // Takes the BGRA32 image data in buffer, and outputs to transformed the grayscale (1 byte per pixel)
        // image data
        private void ProcessImage(byte[] buffer, int width, int height, byte[] transformed)
        {
            if (buffer.Length != transformed.Length * 4)
            {
                Debug.LogError("Invalid buffer size supplied!");
                return;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Gray = (b + g + g + r) / 4
                    byte gray = (byte)((buffer[(y * width * 4) + (4 * x) + 0] +
                                        buffer[(y * width * 4) + (4 * x) + 1] +
                                        buffer[(y * width * 4) + (4 * x) + 1] +
                                        buffer[(y * width * 4) + (4 * x) + 2]) / 4);

                    transformed[y * width + x] = gray;
                }
            }
        }
        

        ~CaptureFrameInstance()
        {
            Marshal.FreeHGlobal(unmanagedFrame);
            Marshal.FreeHGlobal(this.bufPtr);
        }
    }
}
