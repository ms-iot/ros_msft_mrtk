using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This MonoBehaviour is used to toggle between an off state and an on state,
/// both by updating the visuals and enabling the button functionality.
/// </summary>
public class ToggleableButtonController : MonoBehaviour
{

    public MeshRenderer buttonMesh;

    public Material enabledMat;
    public Material disabledMat;

    public Interactable buttonLogic;

    [SerializeField]
    private bool initialState;
    private bool state;

    private void Start()
    {
        if (buttonMesh == null)
        {
            Debug.LogError("CalibrateButtonController is missing a buttonMesh");
        }

        if (enabledMat == null)
        {
            Debug.LogError("CalibrateButtonController is missing a enabledMat");
        }

        if (disabledMat == null)
        {
            Debug.LogError("CalibrateButtonController is missing a disabledMat");
        }

        if (buttonLogic == null)
        {
            Debug.LogError("CalibrateButtonController is missing a buttonLogic");
        }

        UpdateState(initialState);
    }

    /// <summary>
    /// The class performing UI logic (determining when buttons are active) 
    /// needs to call this function to set buttons active/inactive
    /// </summary>
    public void UpdateState(bool state)
    {
        this.state = state;

        buttonMesh.material = state ? enabledMat : disabledMat;

        buttonLogic.enabled = state;
    }
}
