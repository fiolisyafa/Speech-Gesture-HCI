using System;

[Serializable]
public class WeightedUnifiedStructure : ISignal
{
    public UnifiedStructure Data;
    public float Weight;
    public float WeightedConfidence;

    public WeightedUnifiedStructure(UnifiedStructure data, float weight, float confidence)
    {
        this.Data = data;
        this.Weight = weight;
        this.WeightedConfidence = confidence * weight;
    }

    public override string ToString()
    {
        return
        String.Format("weight: {0:0.0000} \ndata: \n", Weight) + Data.ToString();
    }
}