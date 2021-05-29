using System;

[Serializable]
public class Speech : ISignal
{
    public string Result;
    public float Confidence;

    public Speech(string result, float confidence = 0f)
    {
        this.Result = result;
        this.Confidence = confidence;
    }

    public Speech(SpeechData data)
    {
        this.Result = data.Result;
        this.Confidence = data.Confidence;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        Speech other = obj as Speech;
        return other.Result == this.Result;
    }

    public override int GetHashCode()
    {
        return Result.GetHashCode();
    }
}
