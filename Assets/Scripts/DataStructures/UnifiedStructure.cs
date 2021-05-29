using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnifiedStructure : ISignal
{
    public float TimeStamp;
    public string SemanticMeaning;
    public float ConfidenceLevel;
    public string SourceId;

    public float ValidUntil;

    public UnifiedStructure(string sourceId, float timeStamp, string semanticMeaning, float confidenceLevel, float validUntil)
    {
        this.SourceId = sourceId;
        this.TimeStamp = timeStamp;
        this.SemanticMeaning = semanticMeaning;
        this.ConfidenceLevel = confidenceLevel;
        this.ValidUntil = validUntil;
    }

    public override string ToString()
    {
        return
        "sourceId: " + SourceId +
        "\nsemantic: " + SemanticMeaning +
        "\nconfd: " + ConfidenceLevel +
        "\ntime: " + TimeStamp +
        "\nvalid until: " + ValidUntil
        ;
    }
}