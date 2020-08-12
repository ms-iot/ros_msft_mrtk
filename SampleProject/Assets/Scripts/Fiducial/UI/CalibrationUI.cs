using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.WebCam;

public class CalibrationUI : MonoBehaviour
{
    public CalibrateButtonController calibButton;
    public float squareSize;

    private Resolution res;
    private Shader shader;

    private List<WebcamSystem.CaptureFrameInstance> calibImgs;

    private readonly int MINIMUM_CALIBRATION_IMGS = 15;

    // Start is called before the first frame update
    void Start()
    {
        if (calibButton == null)
        {
            Debug.LogError("Calibration button not set on the calibrationUI object");
        }
        if (squareSize == 0)
        {
            Debug.LogError(@"squareSize has not been configured; measure the dimensions 
of a square on your printed checkerboard pattern and input it to the calibrationUI script: " + this);
        }

        calibImgs = new List<WebcamSystem.CaptureFrameInstance>();
    }

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

    public void TakePicture()
    {
        WebcamSystem.instance.CapturePhoto(OnCapturedPhotoToMemory);
    }

    public async void DoCalibration()
    {
        if (calibImgs.Count < MINIMUM_CALIBRATION_IMGS)
        {
            Debug.LogError("Attempted calibration before acquiring the minimum required images!");
        } else
        {
            // Calibration will take a noticeable amount of time, so put it in its own thread
            Task<Intrensics> task = Task.Run(() =>
            {
                Intrensics intr = new Intrensics();
                NativeFiducialFunctions.calibrate(squareSize, out intr);
                return intr;
            });

            // Start UI feedback indicating work is being done...
            
            if (!CameraIntrensicsHelper.WriteIntrensics(await task))
            {
                Debug.LogError("Failed to write intrensics to disk!!");
            }

            // End UI feedback/transition scene

            SceneManager.LoadScene("RobotScene");
        }
    }

    private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame)
    {
        if (result.success)
        {
            Debug.Log("SNAP!!!");
            WebcamSystem.CaptureFrameInstance currFrame = new WebcamSystem.CaptureFrameInstance(frame);
            int prevCount = calibImgs.Count;
            int newCount = NativeFiducialFunctions.supply_calibration_image(currFrame.unmanagedFrame);
            if (newCount > prevCount)
            {
                Debug.Log("Good picture!");
                calibImgs.Add(currFrame);
            } else
            {
                Debug.Log("Couldn't detect the checkerboard. Please try again.");
            }
        }
        else
        {
            Debug.LogError("Unable to take photo!");
        }

    }

    private void Shutdown()
    {
        if (calibImgs.Count > 0)
        {
            calibImgs.Clear();
            NativeFiducialFunctions.clear_calibration_images();
        }
    }
}
