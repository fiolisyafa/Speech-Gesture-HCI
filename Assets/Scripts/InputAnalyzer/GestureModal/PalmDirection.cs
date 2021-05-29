using System;
using Leap.Unity;
using Leap.Unity.Attributes;
using UnityEngine;

[Serializable]
public class PalmDirection : ISignal
{
    [Tooltip("The deviated angle in degrees from the target direction in which the gesture is still valid.")]
    [Range(0, 180)]
    [Units("degree")]
    public float Deviation;

    [Tooltip("The target direction.")]
    [DisableIf("PointingType", isEqualTo: PointingType.AtTarget)]
    public Vector3 PointingDirection = Vector3.forward;

    public PalmDirection(Vector3 pointingDirection, float deviation = 0)
    {
        // Debug.Log(pointingDirection);
        PointingDirection = pointingDirection;
        this.Deviation = deviation;
    }

    public PalmDirection(PalmDirectionData data)
    {
        this.Deviation = data.Deviation;
        this.PointingDirection = data.PointingDirection;
    }

    //TODO: can reuse ToString encoding
    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        PalmDirection pds = obj as PalmDirection;
        if ((object)pds == null) return false;

        string equalsInfo = "self: " + ToString() + "\n other:" + pds.ToString();

        if (pds.PointingDirection == this.PointingDirection)
        {
            LoggerUtil.TmpDebug("pds", "equals: true\n" + equalsInfo + "\n, exact point");
            return true;
        }

        //TODO: gimbal lock?
        float angle = Vector3.Angle(pds.PointingDirection, this.PointingDirection);
        float acceptedDeviation = Math.Max(Deviation, pds.Deviation);

        bool result = angle <= acceptedDeviation;

        string msg = "equals: " + result.ToString() + "\n" +
        "info: " + equalsInfo + "\n" +
        "angle from pointing direction: " + angle + ", deviation " + acceptedDeviation;

        LoggerUtil.TmpDebug("pds", msg);

        return result;
    }

    public override int GetHashCode()
    {
        //TODO: better algorithm

        return 0;
    }

    public override string ToString()
    {
        return "pointingState: " + PointingDirection.ToString() + "\ndeviation: " + Deviation;
    }
}
