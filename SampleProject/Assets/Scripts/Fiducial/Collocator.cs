using ROS2;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Collocator : MonoBehaviour
{
    [Tooltip("The name of the tf frame representing this collocated object e.g. base_link")]
    public string tfFrameName;
    public bool DEBUG_NOISY;

    private static WorldAnchor _anchor;
    private static TransformListener _listener;

    // Update is called once per frame
    void Update()
    {
        if (_anchor != null && tfFrameName != null)
        {
            // If one works, both should
            TfVector3? tfVec = _listener.LookupTranslation("odom", tfFrameName);
            TfQuaternion? tfQuat = _listener.LookupRotation("odom", tfFrameName);
            if (tfVec.HasValue && tfQuat.HasValue)
            {
                Vector3 translationU = VectorHelper.TfToUnity(tfVec.Value);
                Quaternion quatU = new Quaternion((float)tfQuat.Value.x, (float)tfQuat.Value.y, (float)tfQuat.Value.y, (float)tfQuat.Value.w);
                transform.SetPositionAndRotation(_anchor.transform.position + translationU, quatU);
            } else
            {
                Debug.LogWarning("Collocation for " + this.gameObject + " failed because TransformListener.LookupTranslation failed to find translation odom->" + tfFrameName);
            }
        } else if (DEBUG_NOISY)
        {
            Debug.LogWarning("Collocation for " + this.gameObject + " failed because there is no anchor or no frame name was specified.");
        }
        
    }

    /// <summary>
    /// Tell all collocators to start processing their update loops, 
    /// relative to an anchor representing world zero (in ROS space)
    /// </summary>
    /// <param name="anchor">The anchor representing world zero (as understood by ROS)</param>
    public static void StartCollocation(WorldAnchor anchor)
    {
        _anchor = anchor;
        _listener = new TransformListener();
    }
}
