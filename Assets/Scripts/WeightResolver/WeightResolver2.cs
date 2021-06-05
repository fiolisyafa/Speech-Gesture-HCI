
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class WeightResolver2 : MonoBehaviour, ISignalReceiver, ISignalSender<List<WeightedUnifiedStructure>>
// {
//     public ThresholdController2 ThresholdController;
//     private event Action<List<WeightedUnifiedStructure>> OnSignal;

//     public float IdealConfidence = 1f;
//     public bool DebugResult = false;
//     public bool DebugSignal = false;
//     public string DebugTag = "weightResolver";

//     private void Awake()
//     {
//         // ResolverJob = ResolverJob();
//     }

//     private void OnEnable()
//     {
//         ThresholdController.RegisterObserver(UnifiedStructureList_onSignal);
//         //StartCoroutine(ResolverJob);
//     }
//     private void OnDisable()
//     {
//         ThresholdController.UnregisterObserver(UnifiedStructureList_onSignal);
//         //StopCoroutine(ResolverJob);
//     }

//     private List<WeightedUnifiedStructure> Resolve(List<UnifiedStructure> list)
//     {
//         //TODO: annoying undeterministic null behaviour
//         if (list == null || list.Count == 0) return null;

//         float weightConstantDivisor = 0f;
//         foreach (UnifiedStructure item in list)
//         {
//             weightConstantDivisor += (float)Math.Pow(item.ConfidenceLevel, 2);
//         }

//         var msg = "";
//         float weightConstant = IdealConfidence / weightConstantDivisor;
//         if (DebugResult)
//         {
//             msg = "incoming signal: count " + list.Count;
//             msg += "\nideal confidence: " + IdealConfidence;
//             msg += "\nweight constant (K): " + weightConstant + "\n";

//         }

//         List<WeightedUnifiedStructure> weightedList = new List<WeightedUnifiedStructure>();
//         foreach (UnifiedStructure item in list)
//         {
//             weightedList.Add(new WeightedUnifiedStructure(item, item.ConfidenceLevel * weightConstant));
//         }

//         if (DebugResult)
//         {
//             msg += "result: \n";
//             foreach (var item in weightedList)
//             {
//                 msg += "\nweight: " + item.Weight + "\ndata: \n" + item.Data.ToString() + "\n";
//             }

//             LoggerUtil.Log(DebugTag, msg);
//         }

//         return weightedList;
//     }


//     private void UnifiedStructureList_onSignal(List<List<UnifiedStructure>> newList)
//     {
//         //TODO: to be continued from here
//         List<WeightedUnifiedStructure> weightedList = Resolve(newList[0]);

//         if (weightedList != null && weightedList.Count > 0)
//         {
//             if (DebugSignal)
//             {
//                 string debugMsg = "item count: " + weightedList.Count + "\n";
//                 foreach (WeightedUnifiedStructure item in weightedList)
//                 {
//                     debugMsg += item.ToString() + "\n\n";
//                 }
//                 LoggerUtil.Log(DebugTag, debugMsg);
//             }

//             if (OnSignal != null)
//             {
//                 OnSignal(weightedList);
//             }
//         }
//         //TODO: resolve and send directly
//     }

//     private void GroupBySemantic(List<List<UnifiedStructure>> list)
//     {
//         foreach (var aList in list)
//         {
//             aList.OrderByDescending(i => i.SemanticMeaning);
//         }

//         var listByItsLength = list.OrderByDescending(x => x.Count);
//         var longestList = listByItsLength.First();
//         var otherList = listByItsLength.Skip(1);

//         var result = new List<List<WeightedUnifiedStructure>>();
//         foreach (var item in longestList)
//         {
//             var tuple = new List<UnifiedStructure> { item };

//             foreach (var aList in otherList)
//             {
//                 var sameSemantic = aList.Find(x => x.SemanticMeaning == item.SemanticMeaning);
//                 if (sameSemantic != null)
//                 {
//                     tuple.Add(sameSemantic);
//                 }
//             }

//             result.Add(Resolve(tuple));
//         }
//     }

//     void ISignalSender<List<WeightedUnifiedStructure>>.RegisterObserver(Action<List<WeightedUnifiedStructure>> onSignalCallBack)
//     {
//         OnSignal += onSignalCallBack;
//     }

//     void ISignalSender<List<WeightedUnifiedStructure>>.UnregisterObserver(Action<List<WeightedUnifiedStructure>> onSignalCallBack)
//     {
//         OnSignal -= onSignalCallBack;
//     }
// }
