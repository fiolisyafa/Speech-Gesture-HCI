using System;

[Serializable]
public class SpeechEnvironment : ISignal
{
    public string name;
    public float weight;
    public System.Func<float, bool> passes;

    public SpeechEnvironment (string name, float weight, System.Func<float, bool> passes)
    {
        this.name = name;
        this.weight = weight;
        this.passes = passes;
    }

}