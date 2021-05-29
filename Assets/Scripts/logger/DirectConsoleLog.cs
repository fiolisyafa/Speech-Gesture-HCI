
using UnityEngine;

public class DirectConsoleLog : ILogChannel
{
    private string Tag;
    private string Message;
    private string LogSeparator = " - ";
    private enum LogType
    {
        ERROR,
        WARNING,
        LOG
    }

    private LogType Type;

    public void InputError(string tag, string message)
    {
        Process(tag, message, LogType.ERROR);

    }

    public void InputLog(string tag, string message)
    {
        Process(tag, message, LogType.LOG);
    }

    private void Process(string tag, string message, LogType type)
    {
        Type = type;
        Tag = tag;
        Message = message;
        StreamOutput();
    }

    public void StreamOutput()
    {
        switch (Type)
        {
            case LogType.LOG:
                Debug.Log(Tag + LogSeparator + Message);
                break;

            case LogType.ERROR:
                Debug.LogError(Tag + LogSeparator + Message);
                break;

            case LogType.WARNING:
                Debug.LogWarning(Tag + LogSeparator + Message);
                break;
            default:
                Debug.Log(Tag + LogSeparator + Message);
                break;
        }
    }

    public void InputWarning(string tag, string message)
    {
        Process(tag, message, LogType.WARNING);
    }
}