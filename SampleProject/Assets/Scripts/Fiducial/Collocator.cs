using ROS2;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Collocator : MonoBehaviour
{
    [Tooltip("The name of the tf frame representing this collocated object e.g. base_link")]
    public string tfFrameName;
    public bool DEBUG_NOISY;

    private static WorldAnchor _world_anchor;
    private static WorldAnchor _self_anchor;
    private static TransformListener _listener;

    // Update is called once per frame
    void Update()
    {
        if (_world_anchor != null && tfFrameName != null)
        {
            // If one works, both should;
            // odom represents world zero/ _world_anchor
            TfVector3? tfVec = _listener.LookupTranslation("odom", tfFrameName);
            TfQuaternion? tfQuat = _listener.LookupRotation("odom", tfFrameName);
            if (tfVec.HasValue && tfQuat.HasValue)
            {
                Vector3 translationU = TransformHelper.VectorTfToUnity(tfVec.Value);
                Quaternion quatU = new Quaternion((float)tfQuat.Value.x, (float)tfQuat.Value.y, (float)tfQuat.Value.y, (float)tfQuat.Value.w);
                DestroyImmediate(_self_anchor);
                transform.SetPositionAndRotation(_world_anchor.transform.position + translationU, quatU);
                _self_anchor = gameObject.AddComponent<WorldAnchor>();
            } else
            {
                Debug.LogWarning("Collocation for " + this.gameObject + " failed because TransformListener.LookupTranslation failed to find translation odom->" + tfFrameName);
            }
        } else if (DEBUG_NOISY)
        {
            Debug.LogWarning("Collocation for " + this.gameObject + " failed because there is no anchor or no frame name was specified.");
        }
        
    }

    private void Start()
    {
        if (GetComponent<WorldAnchor>() == null)
        {
            _self_anchor = gameObject.AddComponent<WorldAnchor>();
        }
    }

    /// <summary>
    /// Tell all collocators to start processing their update loops, 
    /// relative to an anchor representing world zero (in ROS space)
    /// </summary>
    /// <param name="anchor">The anchor representing world zero (as understood by ROS)</param>
    public static void StartCollocation(WorldAnchor anchor)
    {
        _world_anchor = anchor;
        _listener = new TransformListener();
    }
}
