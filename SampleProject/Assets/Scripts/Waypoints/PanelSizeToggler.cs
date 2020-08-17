using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSizeToggler : MonoBehaviour
{

    public GameObject backplate;

    [SerializeField]
    private GridObjectCollection[] buttonGroups;
    private float[] _panelWidths;
    private GoalPoseClient.GoalPoseClientState _curState;

    public void OnStateChangeCallback(GoalPoseClient.GoalPoseClientState newState)
    {
        if (_panelWidths == null)
        {
            Init();
        }

        backplate.transform.localScale = new Vector3(_panelWidths[(int)newState], backplate.transform.localScale.y, backplate.transform.localScale.z);
        for (int i = 0; i < buttonGroups.Length; i++)
        {
            buttonGroups[i].gameObject.SetActive(false);
        }
        buttonGroups[(int)newState].gameObject.SetActive(true);
    }

    private void Init()
    {
        float anchorContentWidth = buttonGroups[0].CellCount * buttonGroups[0].CellWidth;
        float anchorPanelWidth = backplate.transform.localScale.x;

        _panelWidths = new float[buttonGroups.Length];
        for (int i = 0; i < buttonGroups.Length; i++)
        {
            _panelWidths[i] = anchorPanelWidth * (buttonGroups[i].CellCount * buttonGroups[i].CellWidth) / anchorContentWidth;
        }
    }

}
