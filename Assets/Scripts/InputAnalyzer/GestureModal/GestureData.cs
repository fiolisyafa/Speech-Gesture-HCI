using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: start getting annoying. Use decorator pattern?
//TODO: separation of concern between weight and type data?

[CreateAssetMenu(
    fileName = "GestureData",
    menuName = "ScriptableObjects/GestureData"
)]
public class GestureData : ScriptableObject, ISignal
{
    public ExtendedFingerStateData ExtendedFingerState;
    public float ExtendedFingerStateWeight;
    public PalmDirectionData PalmDirection;
    public float PalmDirectionWeight;
}
