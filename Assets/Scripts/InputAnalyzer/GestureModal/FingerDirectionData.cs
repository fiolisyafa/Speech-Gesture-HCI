using Leap.Unity;
using UnityEngine;

[
    CreateAssetMenu(
        fileName = "FingerDirectionData",
        menuName = "ScriptableObjects/FingerDirectionData"
    )
]

public class FingerDirectionData : ScriptableObject, ISignal
{
    public float Deviation;
    public Vector3[] State = {
        Vector3.forward,
        Vector3.forward,
        Vector3.forward,
        Vector3.forward,
        Vector3.forward,
    };
}