using System.Collections.Generic;
using UnityEngine;

//TODO: more dedicated log support
public static class LoggerUtil
{
    private static bool UseDebugChannel = true;
    public static ILogChannel ResultChannel = new DirectConsoleLog();
    public static ILogChannel DebugChannel = new DirectConsoleLog();
    private static bool TemporaryDebug = true;

    private static List<string> TemporaryDebugTag = new List<string>() { };

    public static void TmpDebug(string tag, string message)
    {
        if (TemporaryDebug && TemporaryDebugTag.Contains(tag))
        {
            if (UseDebugChannel) DebugChannel.InputLog(tag, message);
            ResultChannel.InputLog(tag, message);
        }
    }
    public static void Log(string tag, string message)
    {
        ResultChannel.InputLog(tag, message);
        if (UseDebugChannel)
        {
            DebugChannel.InputLog(tag, message);
        }
    }

    public static void LogError(string tag, string message)
    {
        if (UseDebugChannel) DebugChannel.InputError(tag, message);
        ResultChannel.InputError(tag, message);
    }

    public static void RegisterLogChannel(ILogChannel channel)
    {
        ResultChannel = channel;
    }
}
