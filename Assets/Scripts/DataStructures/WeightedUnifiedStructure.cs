using System;

[Serializable]
public class WeightedUnifiedStructure : ISignal
{
    public UnifiedStructure Data;
    public float Weight;

    public WeightedUnifiedStructure(UnifiedStructure data, float weight)
    {
        this.Data = data;
        this.Weight = weight;
    }

    public override string ToString()
    {
        return
        String.Format("weight: {0:0.0000} \ndata: \n", Weight) + Data.ToString();
    }
}