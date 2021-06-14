using System;

[Serializable]
public class GestureEnvironment : ISignal
{
    public string name;
    public float weight;
    public System.Func<bool, bool, bool> passes;

    public GestureEnvironment (string name, float weight, System.Func<bool, bool, bool> passes)
    {
        this.name = name;
        this.weight = weight;
        this.passes = passes;
    }
}