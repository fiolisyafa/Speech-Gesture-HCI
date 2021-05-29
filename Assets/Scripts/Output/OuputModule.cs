using System.Collections.Generic;
using UnityEngine;

public class OuputModule : MonoBehaviour, ISignalReceiver
{
    public bool DebugResult;
    public bool DebugSignal;
    public string DebugTag = "OutputModule";
    private CollectedLog logChannel;
    public UnificationController UnificationController;

    private void OnEnable()
    {
        logChannel = new CollectedLog();
        LoggerUtil.RegisterLogChannel(logChannel);

        (UnificationController as ISignalSender<List<UnifiedStructure>>)
        .RegisterObserver(FinalUnified_onSignal);
    }

    private void OnDisable()
    {
        (UnificationController as ISignalSender<List<UnifiedStructure>>)
        .UnregisterObserver(FinalUnified_onSignal);
    }

    private void FinalUnified_onSignal(List<UnifiedStructure> newList)
    {
        if (DebugSignal)
        {
            var msg = "received list: \n";
            foreach (var item in newList)
            {
                msg += item.ToString() + "\n";
            }

            LoggerUtil.Log(DebugTag, msg);
        }

        logChannel.StreamOutput();
    }



}