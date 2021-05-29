using Leap;
using Leap.Unity;
using UnityEngine;

public class PalmDirectionWatcherComponent : MonoBehaviour
{
    public bool DebugResult = false;

    public PalmDirection Resolve(Hand hand)
    {
        Vector3 normal = hand.PalmNormal.ToVector3();
        PalmDirection result = new PalmDirection(normal);
        if (DebugResult)
        {
            Debug.Log(result.ToString());
        }

        return result;
    }
}