using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ExtendedFingerStateData",
     menuName = "ScriptableObjects/ExtendedFingerStateData"
    )
]

public class ExtendedFingerStateData : ScriptableObject, ISignal
{
    public PointingState[] State = {
        PointingState.Either,
        PointingState.Either,
        PointingState.Either,
        PointingState.Either,
        PointingState.Either,
        };
}
