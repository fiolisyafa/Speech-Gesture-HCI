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
    public double environmentWeight;

    public UnifiedStructure(string sourceId, float timeStamp, string semanticMeaning, float confidenceLevel, float validUntil, double environmentWeight = 0)
    {
        this.SourceId = sourceId;
        this.TimeStamp = timeStamp;
        this.SemanticMeaning = semanticMeaning;
        this.ConfidenceLevel = confidenceLevel;
        this.ValidUntil = validUntil;
        this.environmentWeight = environmentWeight;
    }

    public override string ToString()
    {
        return string.Format(
            string.Join(
                "\n",
                "source id: {0}",
                "semantic: {1}",
                "confidence: {2}",
                "time: {3}",
                "valid until: {4}",
                "environment weight: {5}"
            ),
            SourceId,
            SemanticMeaning,
            ConfidenceLevel,
            TimeStamp,
            ValidUntil,
            environmentWeight
        );
    }
}