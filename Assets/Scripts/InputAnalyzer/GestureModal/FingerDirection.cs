using System;
using Leap.Unity;
using Leap.Unity.Attributes;
using UnityEngine;

[Serializable]
public class FingerDirection : ISignal
{
    [Tooltip("The deviated angle in degrees from the target direction in which the gesture is still valid.")]
    [Range(0, 180)]
    [Units("degree")]
    public float Deviation;

    public Vector3[] State;

    public FingerDirection(Vector3[] state, float deviation = 0)
    {
        // Debug.Log(pointingDirection);
        this.State = state;
        this.Deviation = deviation;
    }

    public FingerDirection(FingerDirectionData data)
    {
        this.State = data.State;
        this.Deviation = data.Deviation;
    }

    //TODO: can reuse ToString encoding
    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        FingerDirection fds = obj as FingerDirection;
        if ((object)fds == null) return false;

        if (this.State.Length != fds.State.Length) {
            return false;
        }

        for (int i = 0; i < this.State.Length; i++)
        {
            float acceptedDeviation = this.Deviation;
            float angle = Vector3.Angle(this.State[i], fds.State[i]);
            if (angle <= acceptedDeviation) {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        //TODO: better algorithm

        return 0;
    }

    public override string ToString()
    {
        String msg = "";

        foreach (Vector3 item in State)
        {
            msg += item + "\n";
        }

        return msg;
    }
}
