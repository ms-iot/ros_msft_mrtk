using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PanelSizeToggler))]
public class GoalPoseClient : MonoBehaviour
{
    public PanelSizeToggler _panelSizeToggler;

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

    private void Start()
    {
        _panelSizeToggler = GetComponentInChildren<PanelSizeToggler>();
        _poses = new LinkedList<Transform>();
        State = GoalPoseClientState.IDLE;
    }

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

    public void AbortChain()
    {
        Debug.Log("pst = " + _panelSizeToggler);
        Debug.Log("AbortChain()");
        ClearPoses();
        State = GoalPoseClientState.IDLE;
    }

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
        AWAITING = 1
    }
}

