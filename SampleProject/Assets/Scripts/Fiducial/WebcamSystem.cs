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
    public ComputeShader BGRAtoGrayscaleShader;

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
            currFrame = new CaptureFrameInstance(frame, cameraResolution, BGRAtoGrayscaleShader);
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

    private void Shutdown()
    {
        if (captureObject != null && ready)
        {
            ready = false;
            captureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            CaptureFrameInstance.DisposeBuffers();
        }
    }

    /// <summary>
    /// Wrapper for unmanaged copy of image_u8 in memory.
    /// Data is deallocated upon this object going out of scope.
    /// It is the user's responsibility to keep an instance of 
    /// CaptureFrameInstance in-scope until after all native calls
    /// are done using it.
    /// </summary>
    public class CaptureFrameInstance
    {
        public System.IntPtr unmanagedFrame;

        private System.IntPtr _bufPtr;

        // reuse buffers from frame to frame
        private static ComputeBuffer _inputBuffer;
        private static ComputeBuffer _outputBuffer;

        public static void DisposeBuffers()
        {
            if (_inputBuffer != null)
            {
                _inputBuffer.Dispose();
            }
            if (_outputBuffer != null)
            {
                _outputBuffer.Dispose();
            }
        }

        public CaptureFrameInstance(PhotoCaptureFrame managedFrame, Resolution res, ComputeShader sh)
        {
            // build up image struct, copy it to unmanaged memory
            FiducialSystem.image_u8 temp = new FiducialSystem.image_u8();
            temp.width = res.width;
            temp.height = res.height;
            temp.stride = res.width;  // TASK: implement stride optimization 
                                      // in cases where img size not cache-optimized
            temp.buf = Marshal.AllocHGlobal(temp.height * temp.stride);
            _bufPtr = temp.buf;  // stored for easy deallocation later 

            // Obtain managed BGRA image bytes, allocate space for 
            // one-fourth-sized grayscale image bytes (transformedImage)
            List<byte> managedBuffer = new List<byte>();
            managedFrame.CopyRawImageDataIntoBuffer(managedBuffer);
            byte[] transformedImg = new byte[temp.width * temp.height];


            // Do the transformation from BGRA to grayscale, using hardware acceleration if possible
            int kernelHandle = sh.FindKernel("ProcessImage");
            uint groupSize;
            sh.GetKernelThreadGroupSizes(kernelHandle, out groupSize, out _, out _);
            if ((temp.width * temp.height) % groupSize == 0)
            {
                if (_inputBuffer == null)
                {
                    _inputBuffer = new ComputeBuffer(temp.width * temp.height, sizeof(uint));  // bgra values; 4 bytes per pixel
                } 
                if (_outputBuffer == null)
                {
                    _outputBuffer = new ComputeBuffer(temp.width * temp.height / 4, sizeof(uint));  // grayscale values; 1 byte per pixel
                }

                _inputBuffer.SetData(managedBuffer.ToArray());
                sh.SetBuffer(kernelHandle, "In", _inputBuffer);
                sh.SetBuffer(kernelHandle, "Out", _outputBuffer);
                // each group processes groupSize "clumps" of four pixels
                int threadGroupCount = temp.width * temp.height / (int)groupSize / 4;
                sh.Dispatch(kernelHandle, threadGroupCount, 1, 1);
                _outputBuffer.GetData(transformedImg);

            } else
            {
                Debug.LogError("Unusual resolution used-- cannot hardware accelerate! Defaulting to iterative approach...");
                
                ProcessImage(managedBuffer.ToArray(), temp.width, temp.height, transformedImg);
            }

            // Copy the processed image to unmanaged memory
            Marshal.Copy(transformedImg, 0, temp.buf, transformedImg.Length);
            // Allocate the unmanaged image struct
            unmanagedFrame = Marshal.AllocHGlobal(Marshal.SizeOf(temp));
            Marshal.StructureToPtr<FiducialSystem.image_u8>(temp, unmanagedFrame, false);

            FiducialSystem.instance.UpdateSpacePinning(this);
        }

        // Takes the BGRA32 image data in buffer, and outputs to transformed the grayscale (1 byte per pixel)
        // image data. Performed iteratively, use compute shader if possible!
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
            Marshal.FreeHGlobal(_bufPtr);
        }
    }
}
