using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CameraIntrensicsHelper
{
    private static readonly string RELATIVE_SAVE_PATH = "/Intrensics.json";

    public static Intrensics? ReadIntrensics()
    {
        try
        {
            StreamReader reader = new StreamReader(Application.persistentDataPath +
                                                    RELATIVE_SAVE_PATH);
            string json = reader.ReadToEnd();
            if (json.Length == 0)
            {
                return null;
            }

            return JsonUtility.FromJson<Intrensics>(json);
        } catch (IOException e)
        {
            Debug.LogError("File failed to be read:");
            Debug.LogError(e.Message);
            return null;
        }
    }

    public static bool WriteIntrensics(Intrensics intrensics)
    {
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

        return true;
    }

}
