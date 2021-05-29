using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;
using System;

public class ExtendedFingerWatcher : Detector, ISignalSender<ExtendedFingerState>
{
    private static event Action<ExtendedFingerState> OnSignal;

    [Tooltip("interval in seconds to check the state")]
    [Units("seconds")]
    public float period = .1f;

    [Tooltip("The hand model to watch")]
    public HandModelBase handModel = null;

    public bool DebugResult = false;

    private PointingState[] fingersState;
    private IEnumerator watcherCoroutine;

    void Awake()
    {
        //TODO: more readable which finger it is
        fingersState = new PointingState[] {
            PointingState.Either,
            PointingState.Either,
            PointingState.Either,
            PointingState.Either,
            PointingState.Either,
        };

        watcherCoroutine = FingerStateWatcher();
    }

    IEnumerator FingerStateWatcher()
    {
        Hand hand = null;
        while (true)
        {
            if (handModel != null && handModel.IsTracked)
            {
                hand = handModel.GetLeapHand();
                //TODO: hardcoded value
                for (int f = 0; f < 5; f++)
                {
                    Finger finger = hand.Fingers[f];
                    if (finger.IsExtended)
                    {
                        fingersState[f] = PointingState.Extended;
                    }
                    else
                    {
                        fingersState[f] = PointingState.NotExtended;
                    }
                }

                if (OnSignal != null)
                {
                    ExtendedFingerState newState = new ExtendedFingerState(fingersState);
                    if (DebugResult)
                    {
                        Debug.Log(newState.ToString());
                    }
                    OnSignal(newState);

                }
            }
            yield return new WaitForSeconds(period);
        }
    }

    void ISignalSender<ExtendedFingerState>.RegisterObserver(Action<ExtendedFingerState> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    void ISignalSender<ExtendedFingerState>.UnregisterObserver(Action<ExtendedFingerState> onSignalCallBack)
    {
        OnSignal -= onSignalCallBack;
    }

    private void OnEnable()
    {
        StartCoroutine(watcherCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(watcherCoroutine);
    }
}
