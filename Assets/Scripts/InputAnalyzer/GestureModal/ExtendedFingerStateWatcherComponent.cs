using Leap;
using Leap.Unity;
using UnityEngine;

public class ExtendedFingerStateWatcherComponent : MonoBehaviour
{
    public bool DebugResult = false;

    public ExtendedFingerState Resolve(Hand hand, int fingerCount = 5)
    {
        PointingState[] fingersState = new PointingState[fingerCount];

        for (int f = 0; f < fingerCount; f++)
        {
            Finger finger = hand.Fingers[f];
            // if (f == 0) Debug.Log("Finger Direction:" + finger.Direction);
            if (finger.IsExtended)
            {
                fingersState[f] = PointingState.Extended;
            }
            else
            {
                fingersState[f] = PointingState.NotExtended;
            }
        }

        ExtendedFingerState result = new ExtendedFingerState(fingersState);
        if (DebugResult)
        {
            Debug.Log("efs component: " + result.ToString());
        }

        return result;
    }
}