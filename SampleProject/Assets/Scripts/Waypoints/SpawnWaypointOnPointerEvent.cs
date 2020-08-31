using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWaypointOnPointerEvent : MonoBehaviour
{
    private GoalPoseClient gpc;

    private void Start()
    {
        gpc = FindObjectOfType<GoalPoseClient>();
    }

    public void Spawn(MixedRealityPointerEventData eventData)
    {
        gpc.AddPose(eventData.Pointer.Result.Details.Point);
    }
}
