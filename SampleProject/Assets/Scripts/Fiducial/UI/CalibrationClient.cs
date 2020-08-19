using RosSharp.Urdf;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.WebCam;

public class CalibrationClient : MonoBehaviour
{
    public ToggleableButtonController calibButton;
    public GameObject loadingComet;
    public float squareSize;
    [Range(15, 50)]
    public int MINIMUM_CALIBRATION_IMGS = 15;
    public bool DEBUG;

    private Resolution res;
    private Shader shader;

    // We must store the captured frames until the native side is done
    // since their buffers are shared
    private List<WebcamSystem.CaptureFrameInstance> calibImgs;

    

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

        loadingComet.SetActive(false);
        calibImgs = new List<WebcamSystem.CaptureFrameInstance>();
    }

    void Update()
    {
        if (DEBUG && Input.GetKeyDown(KeyCode.P))
        {
            TakePicture();
        }

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
                Debug.Log("Dispatching background thread to perform calibration!");
                Intrensics intr = new Intrensics();
                int success = NativeFiducialFunctions.calibrate(squareSize, out intr);
                return intr;
            });

            // Start UI feedback indicating work is being done...
            loadingComet.SetActive(true);

            Intrensics foundIntr = await task;

            if (!CameraIntrensicsHelper.WriteIntrensics(foundIntr))
            {
                Debug.LogError("Failed to write intrensics to disk!!");
                return;
            }

            // End UI feedback/transition scene
            
            loadingComet.SetActive(false);  // Not strictly needed, since we change scenes
            this.Shutdown();  // Clear the unmanaged memory!
            SceneManager.LoadScene("RobotScene");
        }
    }

    private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame)
    {
        if (result.success)
        {
            Debug.Log("SNAP!!!");

            WebcamSystem.CaptureFrameInstance currFrame = new WebcamSystem.CaptureFrameInstance(frame);

            Debug.LogWarning("dumping img...");
            int res = NativeFiducialFunctions.image_u8_write_pnm(currFrame.unmanagedFrame, "m:\\debugImg\\garbooggle.pnm");

            int prevCount = calibImgs.Count;
            int newCount = NativeFiducialFunctions.supply_calibration_image(currFrame.unmanagedFrame);

            if (newCount > prevCount)
            {
                Debug.Log(string.Format("Good picture! Currently have {0} pictures stored out of the needed {1}.", newCount, MINIMUM_CALIBRATION_IMGS));
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

        calibButton.UpdateState(calibImgs.Count >= MINIMUM_CALIBRATION_IMGS);
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
