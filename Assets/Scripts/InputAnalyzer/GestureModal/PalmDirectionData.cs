using System;
using Leap.Unity;
using Leap.Unity.Attributes;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PalmDirectionData",
    menuName = "ScriptableObjects/PalmDirectionData"
)]
public class PalmDirectionData : ScriptableObject, ISignal
{
    public float Deviation;
    public Vector3 PointingDirection = Vector3.forward;
}
