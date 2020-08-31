using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public partial class WebcamSystem
{
    /// <summary>
    /// Wrapper for unmanaged copy of image_u8 in memory.
    /// Data is deallocated upon this object going out of scope.
    /// It is the user's responsibility to keep an instance of 
    /// CaptureFrameInstance in-scope until after all native calls
    /// are done using it.
    /// </summary>
    public class CaptureFrameInstance
    {
        // built-up native struct of the image. Used by apriltag library.
        public System.IntPtr unmanagedFrame;
        // pointer to the native-side image buffer. Used for easy deallocation later.
        private System.IntPtr _bufPtr;

        // reuse buffers from frame to frame
        // assumes resolution stays constant throughout
        // lifetime of program
        private static ComputeBuffer _inputBuffer;
        private static ComputeBuffer _outputBuffer;
        private static Object bufLock = new Object();

        /// <summary>
        /// The same buffers are shared from instance to instance
        /// to reduce overhead. Consequentially, the program must 
        /// free the buffers manually when done using CaptureFrameInstances
        /// </summary>
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

        public CaptureFrameInstance(PhotoCaptureFrame managedFrame)
        {
            // build up image struct, copy it to unmanaged memory
            image_u8 temp = new image_u8();
            temp.width = WebcamSystem.instance.cameraResolution.width;
            temp.height = WebcamSystem.instance.cameraResolution.height;
            // TASK: implement stride optimization in cases where shape is unoptimized
            // https://docs.microsoft.com/en-us/windows/win32/medfound/image-stride
            temp.stride = WebcamSystem.instance.cameraResolution.width;
            temp.buf = Marshal.AllocHGlobal(temp.height * temp.stride);
            _bufPtr = temp.buf;  // stored for easy deallocation later 

            // Obtain managed BGRA image bytes, allocate space for 
            // one-fourth-sized grayscale image bytes (transformedImage)
            List<byte> managedBuffer = new List<byte>();
            managedFrame.CopyRawImageDataIntoBuffer(managedBuffer);
            byte[] transformedImg = new byte[temp.width * temp.height];


            // Do the transformation from BGRA to grayscale, using hardware acceleration if possible
            int kernelHandle = WebcamSystem.instance.BGRAtoGrayscaleShader.FindKernel("ProcessImage");
            uint groupSize;
            WebcamSystem.instance.BGRAtoGrayscaleShader.GetKernelThreadGroupSizes(kernelHandle, out groupSize, out _, out _);
            if ((temp.width * temp.height) % groupSize == 0)  // TASK: implement stride optimization
            {
                lock(bufLock)
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
                    WebcamSystem.instance.BGRAtoGrayscaleShader.SetBuffer(kernelHandle, "In", _inputBuffer);
                    WebcamSystem.instance.BGRAtoGrayscaleShader.SetBuffer(kernelHandle, "Out", _outputBuffer);
                    // each group processes groupSize "clumps" of four pixels
                    int threadGroupCount = temp.width * temp.height / (int)groupSize / 4;
                    WebcamSystem.instance.BGRAtoGrayscaleShader.Dispatch(kernelHandle, threadGroupCount, 1, 1);
                    _outputBuffer.GetData(transformedImg);
                }
            } else
            {
                Debug.LogError("Unusual resolution used-- cannot hardware accelerate! Defaulting to iterative approach...");
                
                ProcessImage(managedBuffer.ToArray(), temp.width, temp.height, transformedImg);
            }

            // Copy the processed image to unmanaged memory
            Marshal.Copy(transformedImg, 0, temp.buf, transformedImg.Length);
            // Allocate the unmanaged image struct
            unmanagedFrame = Marshal.AllocHGlobal(Marshal.SizeOf(temp));
            Marshal.StructureToPtr<image_u8>(temp, unmanagedFrame, false);
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
