public interface ILogChannel
{
    void InputWarning(string tag, string message);
    void InputError(string tag, string message);
    void InputLog(string tag, string message);
    void StreamOutput();
}