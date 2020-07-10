using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSizeToggler : MonoBehaviour
{

    public GameObject backplate;

    private float[] _panelWidths;
    [SerializeField]
    private GridObjectCollection[] _buttonGroups;
    private GoalPoseClient.GoalPoseClientState _curState;

    public void OnStateChangeCallback(GoalPoseClient.GoalPoseClientState newState)
    {
        if (_panelWidths == null)
        {
            Init();
        }

        backplate.transform.localScale = new Vector3(_panelWidths[(int)newState], backplate.transform.localScale.y, backplate.transform.localScale.z);
        _buttonGroups[(int)_curState].gameObject.SetActive(false);
        _buttonGroups[(int)newState].gameObject.SetActive(true);

    }

    private void Init()
    {
        float anchorContentWidth = _buttonGroups[0].CellCount * _buttonGroups[0].CellWidth;
        float anchorPanelWidth = backplate.transform.localScale.x;

        _panelWidths = new float[_buttonGroups.Length];
        for (int i = 0; i < _buttonGroups.Length; i++)
        {
            _panelWidths[i] = anchorPanelWidth * (_buttonGroups[i].CellCount * _buttonGroups[i].CellWidth) / anchorContentWidth;
        }
    }

}
