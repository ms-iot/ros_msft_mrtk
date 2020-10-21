using ROS2;
using ROS2.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscriptionExample : MonoBehaviour
{
    INode node;
    ISubscription<std_msgs.msg.String> chatter_sub;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            RCLdotnet.Init();
        }
        catch (UnsatisfiedLinkError e)
        {
            Debug.Log(e.ToString());
        }

        node = RCLdotnet.CreateNode ("listener");

        chatter_sub = node.CreateSubscription<std_msgs.msg.String> (
            "chatter", msg => Debug.Log("I heard: [" + msg.Data + "]"), QosProfile.Profile.Default);

    }

    // Update is called once per frame
    void Update()
    {
        RCLdotnet.SpinOnce(node, 0);
    }
}
