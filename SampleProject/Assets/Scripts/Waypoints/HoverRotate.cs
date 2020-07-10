using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverRotate : MonoBehaviour
{
    private readonly float DEVIATION = .1f;
    private readonly float ROTATION_RATE = 6f;

    private Vector3 _anchorPoint;

    // Start is called before the first frame update
    void Start()
    {
        _anchorPoint = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = _anchorPoint + (Vector3.up * Mathf.Sin(Time.time) * DEVIATION);
        transform.localEulerAngles = ( transform.localRotation.eulerAngles + (Vector3.up * Time.deltaTime * ROTATION_RATE));
    }
}
