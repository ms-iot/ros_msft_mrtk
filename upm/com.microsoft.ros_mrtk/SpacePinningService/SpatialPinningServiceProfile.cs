using UnityEngine;
using Microsoft.MixedReality.Toolkit;

[MixedRealityServiceProfile(typeof(ISpatialPinningService))]
[CreateAssetMenu(fileName = "SpatialPinningServiceProfile", menuName = "MixedRealityToolkit/SpatialPinningServiceProfile Configuration Profile")]
public class SpatialPinningServiceProfile : BaseMixedRealityProfile
{
    [SerializeField] 
    [Tooltip("ROS URI Prefix")]
    public string ROSUriPrefix;

    [SerializeField] 
    [Tooltip("ROS Default Namespace")]
    public string ROSNamespace;

    [SerializeField] 
    [Tooltip("ROS Default Domain")]
    public string ROSDefaultDomain;

    [SerializeField] 
    [Tooltip("ROS Default Frame")]
    public string ROSDefaultFrameId;

    [SerializeField] 
    [Tooltip("Use Center of QRCode")]
    public bool UseCenterOfQRCode;
}