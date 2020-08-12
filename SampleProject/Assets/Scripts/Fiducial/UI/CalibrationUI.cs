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

    public void DoCalibration()
    {
        if (calibImgs.Count < MINIMUM_CALIBRATION_IMGS)
        {
            Debug.LogError("Attempted calibration before acquiring the minimum required images!");
        } else
        {
            // Calibration will take a noticeable amount of time, so put it in its own thread
            TaskCompletionSource<Intrensics> tcs = new TaskCompletionSource<Intrensics>();
            Task<Intrensics> task = tcs.Task;
            Task.Factory.StartNew(() =>
            {
                Intrensics intr = new Intrensics();
                NativeFiducialFunctions.calibrate(squareSize, out intr);
                tcs.SetResult(intr);
            });

            // Start UI feedback indicating work is being done...
            
            if (!CameraIntrensicsHelper.WriteIntrensics(task.Result))
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
                calibImgs.Add(currFrame);
            }
        }
        else
        {
            Debug.LogError("Unable to take photo!");
        }

    }

    private void Shutdown()
    {
        NativeFiducialFunctions.clear_calibration_images();
    }
}
