using Leap;
using Leap.Unity;
using UnityEngine;

public class FingerDirectionWatcherComponent : MonoBehaviour
{
    public bool DebugResult = false;

    public FingerDirection Resolve(Hand hand, int fingerCount = 5)
    {
        Vector3[] fingersState = new Vector3[fingerCount];

        for (int f = 0; f < fingerCount; f++)
        {
            Finger finger = hand.Fingers[f];
            fingersState[f] = finger.Direction.ToVector3();
        }

        FingerDirection result = new FingerDirection(fingersState);
        if (DebugResult)
        {
            Debug.Log("fd component: " + result.ToString());
        }

        return result;
    }
}