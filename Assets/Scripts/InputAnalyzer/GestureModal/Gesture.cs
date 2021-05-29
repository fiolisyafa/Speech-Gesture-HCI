using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: start getting annoying. Use decorator pattern?
//TODO: separation of concern between weight and type data?
public class Gesture : ISignal
{
    public ExtendedFingerState ExtendedFingerState;
    public float ExtendedFingerStateWeight;
    public PalmDirection PalmDirection;
    public float PalmDirectionWeight;
    //TODO: guard value more than 1
    public Gesture(
        ExtendedFingerState extendedFingerState = null,
        float extendedFingerStateWeight = 0f,
        PalmDirection palmDirection = null,
        float palmDirectionWeight = 0f
        )
    {
        this.ExtendedFingerState = extendedFingerState;
        this.ExtendedFingerStateWeight = extendedFingerStateWeight;

        this.PalmDirection = palmDirection;
        this.PalmDirectionWeight = palmDirectionWeight;
    }

    public Gesture(GestureData data)
    {
        this.ExtendedFingerState = new ExtendedFingerState(data.ExtendedFingerState);
        this.ExtendedFingerStateWeight = data.ExtendedFingerStateWeight;
        this.PalmDirection = new PalmDirection(data.PalmDirection);
        this.PalmDirectionWeight = data.PalmDirectionWeight;
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        Gesture g = obj as Gesture;

        string equalsInfo = "self: " + ExtendedFingerState.ToString() + " and " + PalmDirection.ToString() + "\n" +
        "other: " + g.ExtendedFingerState.ToString() + " and " + g.PalmDirection.ToString();
        bool efs = this.ExtendedFingerState.Equals(g.ExtendedFingerState);
        bool pds = this.PalmDirection.Equals(g.PalmDirection);

        string msg = "equals: " + equalsInfo + "\n" +
        "efs: " + efs.ToString() + "; pds: " + pds.ToString();

        LoggerUtil.TmpDebug("gesture", msg);

        return efs && pds;
    }

    public bool LenientEquals(object obj)
    {
        if (obj == null) return false;

        Gesture g = obj as Gesture;

        string equalsInfo = "self: " + ExtendedFingerState.ToString() + " and " + PalmDirection.ToString() + "\n" +
        "other: " + g.ExtendedFingerState.ToString() + " and " + g.PalmDirection.ToString();
        bool efs = compareIfNotNullOrTrue(this.ExtendedFingerState, g.ExtendedFingerState);
        bool pds = compareIfNotNullOrTrue(this.PalmDirection, g.PalmDirection);

        string msg = "lenient equals: " + equalsInfo + "\n" +
        "efs: " + efs.ToString() + "; pds: " + pds.ToString();

        LoggerUtil.TmpDebug("gesture", msg);

        return efs || pds;
    }

    public override int GetHashCode()
    {
        //TODO: better algorithm
        int hashCode = 0;

        if (ExtendedFingerState != null)
        {
            hashCode += ExtendedFingerState.GetHashCode();
        }

        if (PalmDirection != null)
        {
            hashCode += PalmDirection.GetHashCode();
        }
        return hashCode;
    }

    private bool compareIfNotNullOrTrue(ISignal self, ISignal other)
    {
        if (self != null && other != null)
        {
            return self.Equals(other);
        }
        else
        {
            return true;
        }
    }

    public override string ToString()
    {
        return
        "efs:" + ToStringIfNotNull(ExtendedFingerState) + "\n" +
        "efs weight: " + ExtendedFingerStateWeight + "\n" +
        "pds:\n" + ToStringIfNotNull(PalmDirection) + "\n" +
        "pds weight: " + PalmDirectionWeight;
    }

    private string ToStringIfNotNull(ISignal signal)
    {
        if (signal == null)
        {
            return "";
        }
        else
        {
            return signal.ToString();
        }

    }


}
