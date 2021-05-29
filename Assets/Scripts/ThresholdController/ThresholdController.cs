using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity;
using Leap.Unity.Attributes;
using UnityEngine;

//TODO: ThresholdController extends signalReceiver but become an active observer instead of waiting for the signal
//TODO: similary, IModal is a hot observable (keep sending signal), but nothing process it
public class ThresholdController : MonoBehaviour, ISignalReceiver, ISignalSender<List<UnifiedStructure>>
{
    [Tooltip("interval in seconds to resolve the captured gesture")]
    [Units("seconds")]
    public float period = .1f;

    public Gesture gesture;

    public List<IModal> Modals;

    private event Action<List<UnifiedStructure>> OnSignal;

    public bool DebugResult = false;
    public bool DebugSignal = false;

    public string DebugTag = "ThresholdController";

    private string case1Log = "Execute new signal that had reached expiry time before entering";
    private string case2Log = "Execute expired stored signal";
    private string case3Log = "Execute stored signal and store new signal";
    private string case4Log = "Executed as every modalities have stored signals";
    //TODO: if we don't keep the signals into queue, we might need to remove this soon
    // private List<List<UnifiedStructure>> ModalSignalList;
    private UnifiedStructure[] SignalOnHold;

    IEnumerator ResolverJob;

    private void Awake()
    {
        InitModals();
        ResolverJob = ThresholdResolver();
    }

    private void OnEnable()
    {
        StartCoroutine(ResolverJob);
    }

    private void OnDisable()
    {
        StopCoroutine(ResolverJob);
    }

    private void OnDestroy()
    {
        Modals.Clear();
        ClearSignalOnHold();
        // ModalSignalList.Clear();
    }

    private void InitModals()
    {
        Modals = new List<IModal>(GetComponentsInChildren<IModal>());
        SignalOnHold = new UnifiedStructure[Modals.Count];


        // ModalSignalList = new List<List<UnifiedStructure>>();
        for (int i = 0; i < Modals.Count; i++)
        {
            SignalOnHold[i] = null;
            // ModalSignalList.Add(new List<UnifiedStructure>());
        }

        if (DebugResult) LoggerUtil.Log(DebugTag, "got " + Modals.Count + " modal(s)");
    }

    /*TODO:
        - threshold resolver should only contain threshold-related logic:
            - hold a signal for a certain threshold before sending it to the next layer
            - decide a signal is to be processed in pair or as a unimodal
                - algorithm:
                - on a signal, keep it as the current
                - on a new signal, if it resolves the same semantic, drop it; if it has different semantic, resolve the latest and save it as a current one
                - after the threshold passes, send it to the next layer either as unimodal input or pair
     */
    IEnumerator ThresholdResolver()
    {
        while (true)
        {
            float currentTime = Time.time;
            for (int modalIdx = 0; modalIdx < Modals.Count; modalIdx++)
            {
                UnifiedStructure currentSignal = null, newSignal = null;
                List<UnifiedStructure> latestSignalList = Modals[modalIdx].GetLatestSignal();
                currentSignal = GetSignalOnHold(modalIdx);
                if (latestSignalList != null)
                {
                    //latestSignalList.RemoveAll(i => i.SemanticMeaning == "default");
                    //newSignal = latestSignalList[0];
                    for(int i = 0; i < latestSignalList.Count; i++){
                        newSignal = latestSignalList[i];
                    }
                }

                //TODO: this three operations might be 3 key problems a threshold controller needs to solve
                //TODO: thus, we can make an abstract class out of it
                if (currentSignal == null && newSignal == null)
                {
                    OnNoSignal(modalIdx, currentTime);
                }
                else if (currentSignal != null && newSignal == null)
                {
                    OnNoNewSignal(currentSignal, modalIdx, currentTime);
                }
                else if (currentSignal == null && newSignal != null)
                {
                    OnNoSignalOnHold(newSignal, modalIdx, currentTime);
                }
                else if (currentSignal != null && newSignal != null)
                {
                    OnSignalOnHoldExist(currentSignal, newSignal, modalIdx, currentTime);
                }
            }

            if (SingalOnHoldIsFull())
            {
                ResolveSignalOnHold(case4Log);
            }

            yield return new WaitForSeconds(period);
        }
    }

    //TODO: find a better word than "duplicate
    private bool IsSameSemantic(int modalIdx, UnifiedStructure newSignal)
    {
        UnifiedStructure currentSignal = SignalOnHold[modalIdx];
        if (currentSignal == null) return false;
        return currentSignal.SemanticMeaning == newSignal.SemanticMeaning;
    }

    private bool IsNewHasBetterConfidence(int modalIdx, UnifiedStructure newSignal)
    {
        UnifiedStructure currentSignal = SignalOnHold[modalIdx];
        if (currentSignal == null) return false;
        return currentSignal.ConfidenceLevel < newSignal.ConfidenceLevel;
    }


    /*private bool IsSignalDuplicate(List<UnifiedStructure> signalList, UnifiedStructure newSignal)
    {
        if (signalList != null && signalList.Count > 0)
        {
            UnifiedStructure latestSignal = signalList[ModalSignalList.Count - 1];
            return newSignal.SemanticMeaning == latestSignal.SemanticMeaning;
        }
        else
        {
            return false;
        }
    }*/

    //TODO: create a separate helper util class
    private bool IsListEmpty<T>(List<T> list)
    {
        return list.Count == 0;
    }

    private UnifiedStructure GetSignalOnHold(int modalIdx)
    {
        return SignalOnHold[modalIdx];
    }

    private void OnNoSignal(int modalIdx, float currentTime)
    {
        if (DebugSignal) LoggerUtil.Log(DebugTag, "threshold: no signal");
        //no signal yet, do nothing
    }

    private void OnNoNewSignal(UnifiedStructure currentSignal, int modalIdx, float currentTime)
    {
        if (currentTime > currentSignal.ValidUntil)
        {
            ResolveSignalOnHold(case2Log);
        }
    }

    private void OnNoSignalOnHold(UnifiedStructure newSignal, int modalIdx, float currentTime)
    {
        if (DebugSignal) LoggerUtil.Log(DebugTag, "threshold: no signal on hold");
        //just register it
        SignalOnHold[modalIdx] = newSignal;

        //no signal in hold
        if (currentTime > newSignal.ValidUntil)
        {
            //resolve it immidiately
            ResolveSignalOnHold(case1Log);
        }
    }

    private void OnSignalOnHoldExist(
        UnifiedStructure currentSignal,
        UnifiedStructure newSignal,
        int modalIdx,
        float currentTime
        )
    {
        bool sameSemantic = IsSameSemantic(modalIdx, newSignal);
        bool betterConfidence = IsNewHasBetterConfidence(modalIdx, newSignal);
        if (sameSemantic && betterConfidence)
        {
            if (DebugResult)
            {
                LoggerUtil.Log(
                    DebugTag,
                     "replacing stored with new signal. Same semantic yet better confidence:\nstored:\n" +
                     SignalOnHold[modalIdx].ToString() +
                     "\n new:\n" + newSignal.ToString()
                    );
            }
            SignalOnHold[modalIdx] = newSignal;
            return;
        }

        if (!sameSemantic)
        {
            ResolveSignalOnHold(case3Log);
            SignalOnHold[modalIdx] = newSignal;
            if (DebugSignal) LoggerUtil.Log(DebugTag, "threshold: signal on hold exists: replacing on hold");
        }
        else if (currentTime > currentSignal.ValidUntil)
        {
            ResolveSignalOnHold(case2Log);
            SignalOnHold[modalIdx] = newSignal;
            if (DebugSignal) LoggerUtil.Log(DebugTag, "threshold: signal on hold exists: replacing on hold because the on hold is expired");
        }
        else
        {
            if (DebugSignal) LoggerUtil.Log(DebugTag, "threshold: duplicated with a valid signal on hold: dropping new one");
        }
    }

    private void ResolveSignalOnHold(string cause)
    {
        List<UnifiedStructure> availableSignal = GetAvailableSignalOnHold();

        string causeMsg = "resolving signal; cause: " + cause + "\n";

        if (availableSignal.Count == 0)
        {
            if (DebugResult) LoggerUtil.Log(DebugTag, causeMsg + "\nno signal to send");
        }
        else if (availableSignal.Count == 1)
        {
            if (DebugResult) LoggerUtil.Log(DebugTag, causeMsg + "\nsending unimodal signal: \n" + availableSignal[0].ToString());
        }
        else
        {
            string semantics = causeMsg + "\nsending " + availableSignal.Count + " multimodal signals: \n";
            for (int i = 0; i < availableSignal.Count; i++)
            {
                semantics += i + " : \n" + availableSignal[i].ToString() + "\n\n";
            }
            if (DebugResult) LoggerUtil.Log(DebugTag, semantics);
        }

        OnSignal(availableSignal);

        ClearSignalOnHold();
    }



    private bool SingalOnHoldIsFull()
    {
        foreach (UnifiedStructure signal in SignalOnHold)
        {
            if (signal == null) return false;
        }

        return true;
    }

    private void ClearSignalOnHold()
    {
        for (int i = 0; i < Modals.Count; i++)
        {
            SignalOnHold[i] = null;
        }
    }

    private List<UnifiedStructure> GetAvailableSignalOnHold()
    {
        List<UnifiedStructure> availableSignal = new List<UnifiedStructure>();

        foreach (UnifiedStructure signal in SignalOnHold)
        {
            if (signal != null)
            {
                availableSignal.Add(signal);
            }
        }

        return availableSignal;
    }

    public void RegisterObserver(Action<List<UnifiedStructure>> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    public void UnregisterObserver(Action<List<UnifiedStructure>> onSignalCallBack)
    {
        OnSignal -= onSignalCallBack;
    }
}
