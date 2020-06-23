using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatterSubscription : MonoBehaviour
{
    private ROS2Listener _r2l;

    // Start is called before the first frame update
    void Start()
    {
        _r2l = ROS2Listener.instance;
        _r2l.node.CreateSubscription<std_msgs.msg.String>(
                "chatter", msg => Debug.Log("I heard: [" + msg.Data + "]"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
