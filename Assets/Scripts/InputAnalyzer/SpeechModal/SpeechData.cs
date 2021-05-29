using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeechData", menuName = "ScriptableObjects/SpeechData")]
public class SpeechData : ScriptableObject, ISignal
{
    public string Result;
    public float Confidence;
}
