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
    public FingerDirection FingerDirection;
    public float FingerDirectionWeight;
    //TODO: guard value more than 1
    public Gesture(
        ExtendedFingerState extendedFingerState = null,
        float extendedFingerStateWeight = 0f,
        PalmDirection palmDirection = null,
        float palmDirectionWeight = 0f,
        FingerDirection fingerDirection = null,
        float fingerDirectionWeight = 0f
        )
    {
        this.ExtendedFingerState = extendedFingerState;
        this.ExtendedFingerStateWeight = extendedFingerStateWeight;

        this.PalmDirection = palmDirection;
        this.PalmDirectionWeight = palmDirectionWeight;
        
        this.FingerDirection = fingerDirection;
        this.FingerDirectionWeight = fingerDirectionWeight;
    }

    public Gesture(GestureData data)
    {
        this.ExtendedFingerState = new ExtendedFingerState(data.ExtendedFingerState);
        this.ExtendedFingerStateWeight = data.ExtendedFingerStateWeight;
        this.PalmDirection = new PalmDirection(data.PalmDirection);
        this.PalmDirectionWeight = data.PalmDirectionWeight;
        this.FingerDirection = new FingerDirection(data.FingerDirection);
        this.FingerDirectionWeight = data.FingerDirectionWeight;
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        Gesture g = obj as Gesture;

        string equalsInfo = String.Format(
            "self: {0} and {1} and {2}\nother: {3} and {4} and {5}",
            ExtendedFingerState,
            PalmDirection,
            FingerDirection,
            g.ExtendedFingerState,
            g.PalmDirection,
            g.FingerDirection
        );

        bool efs = this.ExtendedFingerState.Equals(g.ExtendedFingerState);
        bool pds = this.PalmDirection.Equals(g.PalmDirection);
        bool fd = this.FingerDirection.Equals(g.FingerDirection);

        string msg = String.Format(
            "equals: {0}\nefs: {1}\npds: {2}\n:fd {3}",
            equalsInfo,
            efs,
            pds,
            fd
        );

        LoggerUtil.TmpDebug("gesture", msg);

        return efs && pds;
    }

    public bool LenientEquals(object obj)
    {
        if (obj == null) return false;

        Gesture g = obj as Gesture;

        string equalsInfo = String.Format(
            "self: {0} and {1} and {2}\nother: {3} and {4} and {5}",
            ExtendedFingerState,
            PalmDirection,
            FingerDirection,
            g.ExtendedFingerState,
            g.PalmDirection,
            g.FingerDirection
        );
        bool efs = compareIfNotNullOrTrue(this.ExtendedFingerState, g.ExtendedFingerState);
        bool pds = compareIfNotNullOrTrue(this.PalmDirection, g.PalmDirection);
        bool fd = compareIfNotNullOrTrue(this.FingerDirection, g.FingerDirection);

        string msg = String.Format(
            "equals: {0}\nefs: {1}\npds: {2}\n:fd {3}",
            efs,
            pds,
            fd
        );

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

        if (FingerDirection != null)
        {
            hashCode += FingerDirection.GetHashCode();
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
        return String.Format(
            "efs: {0}\nefs weight: {1}\npds: {2}\npds weight: {3}\nfd: {4}\nfd weight: {5}",
            ToStringIfNotNull(ExtendedFingerState),
            ExtendedFingerStateWeight,
            ToStringIfNotNull(PalmDirection),
            PalmDirectionWeight,
            ToStringIfNotNull(FingerDirection),
            FingerDirectionWeight
        );
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
