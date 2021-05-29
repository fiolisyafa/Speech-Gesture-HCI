
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnificationController : MonoBehaviour, ISignalReceiver, ISignalSender<List<UnifiedStructure>>
{

    private event Action<List<UnifiedStructure>> OnSignal;

    public bool DebugResult;
    public bool DebugSignal;
    public string DebugTag = "UnificationController";
    public string SignalSourceId = "UNIFIED";

    public WeightResolver WeightResolver;
    //TODO: we should better create serializable object system before this

    //TODO: this should be able to search recursively on
    //TODO: how is the data structure, learn more about unification algorithm
    //TODO: definetly still have 1 dimensional array, but with hierarchy. Pointer/ graph perhaps?

    //TODO: new gesture: "swipe right" --> need finger direction
    public Dictionary<String, FeatureStructre> UnificationDatabase;

    void Awake()
    {
    }

    private void ResolveUnificationDatabase()
    {
        //TODO: simplest PoC: produce final confidence level
        //TODO: language is ambiguous. A single recognition could leat to multiple hypothesis. 
        //Is it okay for us to care only the highest confidence? perhaps yes since it is semantic fusion

        /*
        if both has different semantic, would it be merged or select
         */
        /*
    	TODO: consider these fusion cases
    	- both are reliable, with same semantic resolve
    	- both are reliable with different semantic resolve
    	- one are not reliable with same semantic resolve
    	- one are not reliable with different semantic resolve
    	- both are not reliable with same semantic resolve
    	- both are not reliable with different semantic resolve

    	solution:
    	- searching wtih bias
    	- if same semantic, unified it
    	- if different semantic, search recusively for combination

    	**/
    }

    private List<UnifiedStructure> Resolve(List<WeightedUnifiedStructure> list)
    {
        Dictionary<string, List<WeightedUnifiedStructure>> grouped = GroupPerSemantic(list);

        List<UnifiedStructure> result = new List<UnifiedStructure>();
        foreach (KeyValuePair<string, List<WeightedUnifiedStructure>> entry in grouped)
        {
            var resultItem = Merge(entry.Value);
            if (resultItem != null)
            {
                result.Add(resultItem);
            }
        }
        if(result.Count > 1)
        {
            finalSemantic(result);
        }
        return result;
    }

    private Dictionary<string, List<WeightedUnifiedStructure>> GroupPerSemantic(List<WeightedUnifiedStructure> list)
    {
        Dictionary<string, List<WeightedUnifiedStructure>> result = new Dictionary<string, List<WeightedUnifiedStructure>>();
        foreach (WeightedUnifiedStructure item in list)
        {
            if (result.ContainsKey(item.Data.SemanticMeaning))
            {
                List<WeightedUnifiedStructure> currList = result[item.Data.SemanticMeaning];
                currList.Add(item);
            }
            else
            {
                List<WeightedUnifiedStructure> newList = new List<WeightedUnifiedStructure> { item };
                result.Add(item.Data.SemanticMeaning, newList);
            }
        }
        return result;
    }

    private int FindSameSemantics(List<UnifiedStructure> currentList, UnifiedStructure item)
    {
        for (int i = 0; i < currentList.Count; i++)
        {
            if (currentList[i].SemanticMeaning == item.SemanticMeaning)
            {
                return i;
            }
        }
        return -1;

    }

    private UnifiedStructure Merge(List<WeightedUnifiedStructure> list)
    {
        if (list == null || list.Count == 0)
        {
            return null;
        }

        string semantic = list[0].Data.SemanticMeaning;
        float confidence = list[0].Weight;
        float timestamp = list[0].Data.TimeStamp;
        float validUntil = list[0].Data.ValidUntil;
        //string finalSemantic;

        for (int i = 1; i < list.Count; i++)
        {
            WeightedUnifiedStructure item = list[i];
            if (semantic != item.Data.SemanticMeaning) return null;
            if (timestamp > item.Data.TimeStamp) timestamp = item.Data.TimeStamp;
            if (validUntil < item.Data.ValidUntil) validUntil = item.Data.ValidUntil;
            confidence += item.Weight;
        }

        return new UnifiedStructure(
            SignalSourceId,
            timestamp,
            semantic,
            confidence,
            validUntil
        );
    }

    private void WeightedList_onSignal(List<WeightedUnifiedStructure> newList)
    {
        if (newList == null || newList.Count == 0)
        {
            if (DebugSignal) LoggerUtil.Log(DebugTag, "empty signal");
            return;
        }

        if (DebugSignal)
        {
            var msg = "signal count: " + newList.Count + "\n\n";
            foreach (WeightedUnifiedStructure item in newList)
            {
                msg += item.ToString() + "\n\n";
            }
            LoggerUtil.Log(DebugTag, msg);
        }

        var result = Resolve(newList);
        if (DebugResult)
        {
            var msg = "unified into " + result.Count + " items\n";
            foreach (var item in result)
            {
                msg += item.ToString() + "\n\n";
            }
            LoggerUtil.Log(DebugTag, msg);
        }

        OnSignal(result);
    }

    /* core feature of the framework --> reliability on the other modal
    if both modals have different semantic, take only the one with higher confidence
    if both modals have different semantic but same confidence, take the one with the earlier timestamp */
    private void finalSemantic(List<UnifiedStructure> list)
    {
        //performance overkill
        string msg = "choosing final semantic\n\nfinal semantic:\n\n";
        //float finalConf;
        for(int i = 0; i<list.Count;i++)
        {
            for(int j = i+1; j<list.Count;j++)
            {
                if(list[i].SemanticMeaning!=list[j].SemanticMeaning)
                {
                    if (list[i].ConfidenceLevel == list[j].ConfidenceLevel)
                    {
                        /* if both signals have different semantics but same confidence level, the signal with the
                        earliest timestamp will be taken as the final semantic.

                        the variable newList stores the signal with earlier timestamp

                        aggregate function =  if item1 have earlier timestamp, take item1, otherwise take item2 */
                        var newList = list.Aggregate((item1, item2) => item1.TimeStamp < item2.TimeStamp ? item1 : item2);
                        msg += newList.ToString() + "\n\nreason: taking signal with earlier timestamp\n\n";

                        //remove the signal with longer timestamp
                        list.RemoveAll(x => x.TimeStamp > newList.TimeStamp);
                    }
                    else
                    {
                        /* if both signals have different semantic AND confidence level, the signal with the higher confidence level
                        will be taken as the final semantic.

                        the variable newList stores the signal with higher confd value

                        aggregate function = if items1 have higher confidence, take items1, otherwise take items2 */
                        var newList = list.Aggregate((items1, items2) => items1.ConfidenceLevel > items2.ConfidenceLevel 
                                                      ? items1 : items2);
                        msg += newList.ToString() + "\n\nreason: taking signal with higher confidence level\n\n";

                        //remove the signal with lower confd level
                        list.RemoveAll(x => x.ConfidenceLevel < newList.ConfidenceLevel);
                    }
                }
                    LoggerUtil.Log(DebugTag, msg);
            }
        }
    }
    private void OnEnable()
    {
        (WeightResolver as ISignalSender<List<WeightedUnifiedStructure>>).RegisterObserver(WeightedList_onSignal);
    }

    private void OnDisable()
    {
        (WeightResolver as ISignalSender<List<WeightedUnifiedStructure>>).UnregisterObserver(WeightedList_onSignal);
    }

    void ISignalSender<List<UnifiedStructure>>.RegisterObserver(Action<List<UnifiedStructure>> onSignalCallBack)
    {
        OnSignal += onSignalCallBack;
    }

    void ISignalSender<List<UnifiedStructure>>.UnregisterObserver(Action<List<UnifiedStructure>> onSignalCallBack)
    {
        OnSignal -= onSignalCallBack;
    }
}
