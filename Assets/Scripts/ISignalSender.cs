using System;

public interface ISignalSender<T>
{
    void RegisterObserver(Action<T> onSignalCallBack);
    void UnregisterObserver(Action<T> onSignalCallBack);
}