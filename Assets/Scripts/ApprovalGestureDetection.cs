using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class ApprovalGestureDetection : MonoBehaviour
{

    public DetectorLogicGate gesture;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gesture.IsActive)
        {
            Debug.Log("approval gesture detection ACTIVE");

        }
    }
}
