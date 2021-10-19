using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.WorldLocking.Core;
using System;
using System.Threading.Tasks;

[AddComponentMenu("Scripts/ROS/HandMenuHandler")]
public class ROSHandMenuHandler : MonoBehaviour
{
    public void OnClick()
    {
        ISpatialPinningService spacePinningService = MixedRealityToolkit.Instance.GetService<ISpatialPinningService>();
        spacePinningService?.Calibrate();
    }
}
