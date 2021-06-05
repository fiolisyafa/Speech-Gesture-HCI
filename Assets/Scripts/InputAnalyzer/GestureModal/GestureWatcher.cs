using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;
using System;

public class GestureWatcher : Detector, ISignalSender<Gesture>
{
    private static event Action<Gesture> OnSignal;

    [Tooltip("interval in seconds to check the state")]
    [Units("seconds")]
    public float period = .3f;

    [Tooltip("The hand model to watch")]
    public HandModelBase handModel = null;

    public bool DebugResult = false;
    public string DebugTag = "GestureWatcher";

    private IEnumerator watcherCoroutine;

    public ExtendedFingerStateWatcherComponent EfsComponent;
    public PalmDirectionWatcherComponent PdComponent;
    

    void Awake()
    {
        watcherCoroutine = WatcherJob();
    }

    private IEnumerator WatcherJob()
    {
        Hand hand = null;
        while (true)
        {
            if (handModel != null && handModel.IsTracked)
            {
                Gesture g = null;
                hand = handModel.GetLeapHand();
                if (hand != null)
                {
                    ExtendedFingerState efs = EfsComponent.Resolve(hand);
                    PalmDirection pd = PdComponent.Resolve(hand);

                    // LoggerUtil.Log(DebugTag, pd.ToString());
                    // LoggerUtil.Log(DebugTag, efs.ToString());

                    //TODO: separation of concern between weight and type data?
                    g = new Gesture(extendedFingerState: efs, palmDirection: pd);
                    if (DebugResult)
                    {
                        LoggerUtil.Log(DebugTag, "gesture: " + g.ToString());
                    }
                }

                if (OnSignal != null && g != null)
                {
                    OnSignal(g);
                }
            }
            yield return new WaitForSeconds(period);
        }
    }

    void ISignalSender<Gesture>.RegisterObserver(Action<Gesture> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    void ISignalSender<Gesture>.UnregisterObserver(Action<Gesture> onSignalCallBack)
    {
        OnSignal -= onSignalCallBack;
    }

    private void OnEnable()
    {
        EfsComponent = GetComponents<ExtendedFingerStateWatcherComponent>()[0];
        PdComponent = GetComponents<PalmDirectionWatcherComponent>()[0];

        if (EfsComponent == null || PdComponent == null)
        {
            Debug.LogError("cannot start gesture watcher with incomplete watcher components");
        }
        else
        {
            StartCoroutine(watcherCoroutine);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(watcherCoroutine);
    }
}
