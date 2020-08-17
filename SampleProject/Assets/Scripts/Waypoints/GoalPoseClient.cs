using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PanelSizeToggler))]
public class GoalPoseClient : MonoBehaviour
{
    public PanelSizeToggler _panelSizeToggler;


    [SerializeField]
    private GoalPoseClient.GoalPoseClientState initialActiveGroup;
    private LinkedList<Transform> _poses;
    private bool _plotting = false;
    private GoalPoseClientState _state;


    public bool Plotting
    {
        get { return _plotting; }
        set { _plotting = value; }
    }

    public GoalPoseClientState State
    {
        get { return _state; }
        set
        {
            _state = value;
            if (_panelSizeToggler != null)
            {
                _panelSizeToggler.OnStateChangeCallback(_state);
            }
        }
    }

    private void Awake()
    {
        State = initialActiveGroup;
    }

    private void Start()
    {
        _panelSizeToggler = GetComponentInChildren<PanelSizeToggler>();
        _poses = new LinkedList<Transform>();
    }

    #region ChainBuildingGroup

    public void StartChain()
    {
        Debug.Log("StartChain()");
        _plotting = true;
    }

    public void CancelChain()
    {
        Debug.Log("CancelChain()");
        ClearPoses();
    }

    public void SendChain()
    {
        Debug.Log("SendChain()");
        // Task: Start an action exchange with the ros action server
        State = GoalPoseClientState.AWAITING;  // (if action server accepts request)
        Debug.Log("pst = " + _panelSizeToggler);

    }

    #endregion  // ChainBuildingGroup
    #region ChainWatchingGroup

    public void AbortChain()
    {
        Debug.Log("pst = " + _panelSizeToggler);
        Debug.Log("AbortChain()");
        ClearPoses();
        State = GoalPoseClientState.IDLE;
    }

    #endregion  // ChainWatchingGroup
    #region CalibrationPromptGroup

    public void GotoCalibration()
    {
        SceneManager.LoadScene("CalibrationScene");
    }

    #endregion  // CalibrationPromptGroup

    private void ClearPoses()
    {
        foreach (Transform t in _poses)
        {
            Destroy(t);
        }
        _poses.Clear();
    }

    public enum GoalPoseClientState
    {
        IDLE = 0,
        AWAITING = 1,
        NEEDING_CALIBRATION = 2
    }
}

