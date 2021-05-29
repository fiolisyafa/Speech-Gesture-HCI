using System.Collections.Generic;
using UnityEngine;

class CollectedLog : ILogChannel
{
    private string LogSeparator = " - ";
    private enum LogType
    {
        ERROR,
        WARNING,
        LOG
    }

    private List<KeyValuePair<LogType, string>> Data = new List<KeyValuePair<LogType, string>>();

    public void InputError(string tag, string message)
    {
        Process(tag, message, LogType.ERROR);
    }

    public void InputLog(string tag, string message)
    {
        Process(tag, message, LogType.LOG);
    }

    public void StreamOutput()
    {
        var outputMsg = "Beginning of Log\n";
        var logDivider = "\n------------------------\n";
        foreach (var item in Data)
        {
            outputMsg += logDivider + item.Value;
        }
        Data.Clear();
        Debug.Log(outputMsg);
    }

    public void InputWarning(string tag, string message)
    {
        Process(tag, message, LogType.WARNING);
    }

    private void Process(string tag, string message, LogType type)
    {
        var newItem = new KeyValuePair<LogType, string>(type, tag + LogSeparator + message);
        Data.Add(newItem);
    }
}