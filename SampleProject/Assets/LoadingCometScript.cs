using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCometScript : MonoBehaviour
{
    public float speed;
    public TrailRenderer tr;

    private Vector3 localOrigin;

    // Start is called before the first frame update
    void Start()
    {
        localOrigin = transform.localPosition;
        tr.widthMultiplier = transform.localScale.x;

        if (speed < 0f)
        {
            Debug.LogWarning("Bad speed value detected in " + this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = localOrigin + (transform.up * transform.localScale.x * Mathf.Cos(Time.time * speed)) + 
            (transform.right * transform.localScale.x * Mathf.Sin(Time.time * speed));
        
    }
}
