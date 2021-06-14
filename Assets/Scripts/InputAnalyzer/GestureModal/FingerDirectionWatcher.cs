using System.Collections;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;
using System;

public class FingerDirectionWatcher : Detector, ISignalSender<FingerDirection>
{
    private static event Action<FingerDirection> OnSignal;

    [Tooltip("interval in seconds to check the state")]
    [Units("seconds")]
    public float period = .1f;

    [Tooltip("The hand model to watch")]
    public HandModelBase handModel = null;

    public bool DebugResult = false;

    private Vector3[] fingerDirection;
    private IEnumerator watcherCoroutine;

    void Awake()
    {
        //TODO: more readable which finger it is
        fingerDirection = new Vector3[] {
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
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
                    fingerDirection[f] = finger.Direction.ToVector3();
                }

                Debug.Log(fingerDirection);

                if (OnSignal != null)
                {
                    FingerDirection newState = new FingerDirection(fingerDirection);
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

    void ISignalSender<FingerDirection>.RegisterObserver(Action<FingerDirection> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    void ISignalSender<FingerDirection>.UnregisterObserver(Action<FingerDirection> onSignalCallBack)
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
