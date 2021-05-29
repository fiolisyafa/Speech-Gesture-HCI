using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Attributes;
using UnityEngine;

public class PalmDirectionResolver : MonoBehaviour, IModal, ISignalReceiver
{

    [Units("seconds")]
    [Tooltip("threshold")]
    public float threshold = 4f;

    [Tooltip("interval in seconds to resolve the captured gesture")]
    [Units("seconds")]
    public float period = .3f;
    public bool DebugResult = false;
    public string SignalSourceId = "pds test";

    public ISignalSender<PalmDirection> PalmDirectionWatcher;
    private Queue<PalmDirection> States = new Queue<PalmDirection>();
    private event Action<UnifiedStructure> OnSignal;

    private UnifiedStructure LatestSignal;

    IEnumerator ResolverJob;
    public SignalDatabase GestureDatabase;
    //TODO: Load from external database source
    Dictionary<Gesture, string> Database = new Dictionary<Gesture, string>();

    private void Awake()
    {
        ResolveSignalSenders();
        ResolverJob = PalmDirectionResolverJob();

        PalmDirection approvalPalmDirectionState =
        new PalmDirection(new Vector3(1.0f, 0.5f, 0.5f), 90f);

        /*Gesture approval = new Gesture(palmDirection: approvalPalmDirectionState);
        Database.Add(approval, "approve");*/

    }

    private void OnEnable()
    {
        PalmDirectionWatcher.RegisterObserver(PalmDirectionWatcher_onSignal);
        StartCoroutine(ResolverJob);
    }

    private void OnDisable()
    {
        PalmDirectionWatcher.UnregisterObserver(PalmDirectionWatcher_onSignal);
        StopCoroutine(ResolverJob);
    }

    private void ResolveSignalSenders()
    {
        PalmDirectionWatcher =
            GetComponents<ISignalSender<PalmDirection>>()[0] as ISignalSender<PalmDirection>;
    }

    public List<UnifiedStructure> GetLatestSignal()
    {
        throw new NotImplementedException();
    }

    public void RegisterObserver(Action<List<UnifiedStructure>> onSignalCallBack)
    {
        throw new NotImplementedException();
    }

    public void UnregisterObserver(Action<List<UnifiedStructure>> onSignalCallBack)
    {
        throw new NotImplementedException();
    }

    IEnumerator PalmDirectionResolverJob()
    {
        while (true)
        {
            if (States.Count > 0)
            {
                PalmDirection currentState = States.Dequeue();
                LatestSignal = Resolve(new Gesture(palmDirection: currentState));

                if (LatestSignal != null)
                {
                    if (DebugResult) Debug.Log("gesture: " + LatestSignal.ToString());
                }
            }

            yield return new WaitForSeconds(period);
        }

    }

    private UnifiedStructure Resolve(Gesture gesture)
    {
        float currentTime = Time.time;
        //TODO: value comparison?
        if (gesture != null)
        {
            Debug.Log("search for " + gesture.ToString());
            if (Database.ContainsKey(gesture))
            {
                string semantic = Database[gesture];
                return new UnifiedStructure(SignalSourceId, currentTime, semantic, 1f, currentTime + threshold);
            }
            else
            {
                Debug.Log("gesture " + gesture + " entry not found");
                return null;
            }
        }
        else
        {
            Debug.Log("gesture resolver, gesture is empty");
            return null;
        }
    }

    private void PalmDirectionWatcher_onSignal(PalmDirection newState)
    {
        States.Enqueue(newState);
    }
}
