using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CameraIntrensicsHelper
{
    private static readonly string RELATIVE_SAVE_PATH = "/Intrensics.json";

    private static Intrensics? intr;

    /// <summary>
    /// Retrieve the stored intrensics values from disk. Caches the value to avoid subsequent file i/o
    /// If for some reason you need to read -> write -> read new value, you must first call ClearIntrensicsCache()
    /// </summary>
    /// <returns>The camera calibration intrensics, or null on failure</returns>
    public static Intrensics? ReadIntrensics()
    {
        // Avoid file i/o if possible, using cached value
        if (intr.HasValue)
        {
            return intr.Value;
        }

        try
        {
            Debug.Log(Application.persistentDataPath);
            StreamReader reader = new StreamReader(Application.persistentDataPath +
                                                    RELATIVE_SAVE_PATH);
            string json = reader.ReadToEnd();
            if (json.Length == 0)
            {
                return null;
            }

            Intrensics output = JsonUtility.FromJson<Intrensics>(json);
            Debug.Log(string.Format("Successfuly read intrensics from disk; fx:{0} fy:{1} cx:{2} cy:{3}", output.fx, output.fy, output.cx, output.cy));
            intr = output;
            return output;
        } catch (IOException e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    /// <summary>
    /// Clears the caches intrensics value, forcing the next read call to go to disk.
    /// </summary>
    public static void ClearIntrensicsCache()
    {
        intr = null;
    }

    /// <summary>
    /// Writes the given intrensic values to disk.
    /// </summary>
    /// <param name="intrensics">The values to be written to disk</param>
    /// <returns>true if the disk operation succeeded, otherwise false</returns>
    public static bool WriteIntrensics(Intrensics intrensics)
    {
        Debug.Log(string.Format("Writing intrensics to disk; fx:{0} fy:{1} cx:{2} cy:{3}", intrensics.fx, intrensics.fy, intrensics.cx, intrensics.cy));


        string json = JsonUtility.ToJson(intrensics);
        try
        {
            StreamWriter writer = new StreamWriter(Application.persistentDataPath +
                                                RELATIVE_SAVE_PATH, false);
            writer.Write(json);
            writer.Flush();
            writer.Close();
        } catch (IOException e)
        {
            Debug.LogError("File failed to be written:");
            Debug.LogError(e.Message);
            return false;
        }

        intr = intrensics;
        return true;
    }

}
