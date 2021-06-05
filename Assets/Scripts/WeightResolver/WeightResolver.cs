
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeightResolver : MonoBehaviour, ISignalReceiver, ISignalSender<List<WeightedUnifiedStructure>>
{
    public ThresholdController ThresholdController;
    private event Action<List<WeightedUnifiedStructure>> OnSignal;

    public float IdealConfidence = 1f;
    public bool DebugResult = false;
    public bool DebugSignal = false;
    public string DebugTag = "weightResolver";

    private void Awake()
    {
        // ResolverJob = ResolverJob();
    }

    private void OnEnable()
    {
        ThresholdController.RegisterObserver(UnifiedStructureList_onSignal);
        //StartCoroutine(ResolverJob);
    }
    private void OnDisable()
    {
        ThresholdController.UnregisterObserver(UnifiedStructureList_onSignal);
        //StopCoroutine(ResolverJob);
    }

    private List<WeightedUnifiedStructure> Resolve(List<UnifiedStructure> list)
    {
        //TODO: annoying undeterministic null behaviour
        if (list == null || list.Count == 0) return null;

        float weightConstantDivisor = 0f;
        foreach (UnifiedStructure item in list)
        {
            weightConstantDivisor += item.ConfidenceLevel;
        }

        // weight constant (K) is calculated only in multimodal mode, otherwise it is 1 by default
        // simiar to if list contains more than 1 value K would be ideal confd/weight divisor, else K = 1
        var weightConstant = (list.Count > 1) ? IdealConfidence / weightConstantDivisor : 1;
        var msg = "";
        if (DebugResult)
        {
            msg = "incoming signal: count " + list.Count;
            if(list.Count == 1) msg += "\nsignal is unimodal: no weighting needed\n";
            msg += "\nideal confidence: " + IdealConfidence;
            msg += "\nweight constant (K): " + weightConstant + "\n";

        }

        List<WeightedUnifiedStructure> weightedList = new List<WeightedUnifiedStructure>();
        foreach (UnifiedStructure item in list)
        {
            weightedList.Add(new WeightedUnifiedStructure(item, item.ConfidenceLevel * weightConstant, item.ConfidenceLevel));
        }

        if (DebugResult)
        {
            msg += "result: \n";
            foreach (var item in weightedList)
            {
                msg += String.Format("\nmodal weight: {0}\nmodal weighted confidence: {1}\ndata: {2}\n\n", item.Weight, item.WeightedConfidence, item.Data.ToString());
            }

            LoggerUtil.Log(DebugTag, msg);
        }

        return weightedList;
    }

    private void UnifiedStructureList_onSignal(List<UnifiedStructure> newList)
    {
        List<WeightedUnifiedStructure> weightedList = Resolve(newList);

        if (weightedList != null && weightedList.Count > 0)
        {
            if (DebugSignal)
            {
                string debugMsg = "item count: " + weightedList.Count + "\n";
                foreach (WeightedUnifiedStructure item in weightedList)
                {
                    debugMsg += item.ToString() + "\n\n";
                }
                LoggerUtil.Log(DebugTag, debugMsg);
            }

            if (OnSignal != null)
            {
                OnSignal(weightedList);
            }
        }
        //TODO: resolve and send directly

    }

    void ISignalSender<List<WeightedUnifiedStructure>>.RegisterObserver(Action<List<WeightedUnifiedStructure>> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    void ISignalSender<List<WeightedUnifiedStructure>>.UnregisterObserver(Action<List<WeightedUnifiedStructure>> onSignalCallBack)
    {
        OnSignal -= onSignalCallBack;
    }
}
