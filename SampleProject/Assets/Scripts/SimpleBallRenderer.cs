using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRenderer : ISpaceRenderer
{

    /// <summary>
    /// Prefab that is instantiated as the "ball"
    /// Used for this renderer's visualization.
    /// Currently Resource.Loaded from root of resources folder
    /// </summary>
    protected GameObject _ballPrefab;
    protected GameObject[] _frame;

    public BallRenderer()
    {
        _ballPrefab = Resources.Load<GameObject>("Sphere");

        if (_ballPrefab == null)
        {
            Debug.LogError("BallRenderer failed to locate the ball prefab!");
        }
    }

    
    public virtual void Render(float[] lidarData, Transform origin)
    {
        if (_frame == null)
        {
            _frame = new GameObject[lidarData.Length];
        }
        else if (lidarData.Length != _frame.Length)
        {
            Debug.LogError("The passed in set of data to render varied from frame to frame... should this be supported?");
            return;
        }

        // delete the old objects from the previous frame
        foreach (GameObject g in _frame)
        {
            GameObject.Destroy(g);
        }

        for (int i = 0; i < lidarData.Length; i++)
        {
            GameObject ball = GameObject.Instantiate(_ballPrefab, origin);
            _frame[i] = ball;
            float rad = ((float)i / (float)lidarData.Length) * (2 * Mathf.PI);
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * lidarData[i];  // offset by 90 degrees so that first data point corresponds to x axis/straight ahead
            ball.transform.localPosition = offset;
        }
    }
}
