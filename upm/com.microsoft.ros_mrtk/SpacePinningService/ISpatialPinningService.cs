using System;
using ROS2;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.QR;

public delegate void QRCodeFunction(QRCode qrCode);

public interface ISpatialPinningService : IMixedRealityExtensionService
{

    event EventHandler OnROSWorldPinned;
    event QRCodeFunction OnQRAdded;
    event QRCodeFunction OnQRUpdated;
    event QRCodeFunction OnQRRemoved;
    event QRCodeFunction OnQREnumerated;

    string URIPrefix { get; }
    string DefaultFrameId { get; }

    TransformListener SharedListener {get;}

    void Calibrate();
}
