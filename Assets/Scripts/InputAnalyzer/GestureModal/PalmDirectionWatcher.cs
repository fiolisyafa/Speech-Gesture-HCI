using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;
using UnityEngine;

//TODO: perhaps the watchers don't need to be child class of detector anymore
public class PalmDirectionWatcher : Detector, ISignalSender<PalmDirection>
{
    private static event Action<PalmDirection> OnSignal;

    [Units("seconds")]
    [Tooltip("The interval in seconds at which to check this detector's conditions.")]
    [MinValue(0)]
    public float Period = .1f; //seconds

    [Tooltip("The hand model to watch. Set automatically if detector is on a hand.")]
    public HandModelBase HandModel = null;

    public bool DebugResult = false;

    [Header("Direction Settings")]
    [Tooltip("How to treat the target direction.")]
    public PointingType PointingType = PointingType.RelativeToHorizon;

    private IEnumerator WatcherCoroutine;

    private void Awake()
    {
        WatcherCoroutine = PalmWatcherJob();
    }

    private void OnEnable()
    {
        StartCoroutine(WatcherCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(WatcherCoroutine);
    }

    public void RegisterObserver(Action<PalmDirection> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    public void UnregisterObserver(Action<PalmDirection> onSignalCallBack)
    {
        OnSignal -= onSignalCallBack;
    }

    private IEnumerator PalmWatcherJob()
    {
        Hand hand;
        Vector3 normal;

        while (true)
        {
            if (HandModel != null && HandModel.IsTracked)
            {
                hand = HandModel.GetLeapHand();
                if (hand != null)
                {

                    normal = hand.PalmNormal.ToVector3();
                    PalmDirection newSignal = new PalmDirection(normal);
                    if (DebugResult)
                    {
                        Debug.Log("new signal: " + newSignal.ToString());
                        // OnSignal(newSignal);
                    }
                }
            }
            yield return new WaitForSeconds(Period);
        }
    }


    private Vector3 SelectedDirection(Vector3 handNormal)
    {
        switch (PointingType)
        {
            case PointingType.RelativeToHorizon:
                Quaternion cameraRot = Camera.main.transform.rotation;
                float cameraYaw = cameraRot.eulerAngles.y;
                Quaternion rotator = Quaternion.AngleAxis(cameraYaw, Vector3.up);
                return rotator * handNormal;
            case PointingType.RelativeToCamera:
                return Camera.main.transform.TransformDirection(handNormal);
            case PointingType.RelativeToWorld:
                return handNormal;
            default:
                return handNormal;
        }
    }
}
