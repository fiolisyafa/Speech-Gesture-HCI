using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

public class ExtendedFingerState : ISignal
{
    public PointingState[] State;

    public ExtendedFingerState(PointingState[] pointingState)
    {
        this.State = pointingState;
    }

    public ExtendedFingerState(ExtendedFingerStateData data)
    {
        this.State = data.State;
    }
    //TODO: can reuse ToString encoding
    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        ExtendedFingerState efs = obj as ExtendedFingerState;

        LoggerUtil.TmpDebug("efs", "equals:\n self: " + ToString() + "\n other:" + efs.ToString());

        if ((object)efs == null) return false;
        if (State.Length == efs.State.Length)
        {
            for (int i = 0; i < State.Length; i++)
            {
                if (State[i] == PointingState.Either || efs.State[i] == PointingState.Either)
                {
                    LoggerUtil.TmpDebug("efs", "either, continue");
                    continue;
                }
                if (State[i] != efs.State[i])
                {
                    LoggerUtil.TmpDebug("efs", "equals false");
                    return false;
                }
            }
            LoggerUtil.TmpDebug("efs", "equals true");

            return true;
        }
        else
        {
            LoggerUtil.TmpDebug("efs", "different size");
            return false;
        }
    }

    public override int GetHashCode()
    {
        //TODO: better algorithm
        return 0;
    }

    //TODO: create gizomos
    public override string ToString()
    {
        String msg = "";
        for (int i = 0; i < State.Length; i++)
        {
            if (State[i] == PointingState.Extended)
            {
                msg += "E";
            }
            else if (State[i] == PointingState.NotExtended)
            {
                msg += "O";
            }
            else
            {
                msg += "?";
            }
        }
        return msg;
    }

}
