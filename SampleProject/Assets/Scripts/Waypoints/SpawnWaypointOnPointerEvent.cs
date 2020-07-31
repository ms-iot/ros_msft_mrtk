using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWaypointOnPointerEvent : MonoBehaviour
{
    public GameObject prefab;

    public void Spawn(MixedRealityPointerEventData eventData)
    {
                                   if (prefab != null)
        {
            Debug.Log("Spawning waypoint prefab");
            Instantiate(prefab, eventData.Pointer.Result.Details.Point, Quaternion.identity);
        } else
        {
            Debug.LogWarning("Prefab for waypoint spawner not given!");
        }
    }
}
