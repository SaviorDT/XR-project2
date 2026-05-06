using System;
using UnityEngine;
using UnityEngine.XR;

public class KeyInputController : MonoBehaviour
{
    private Action<TempoEventType> callback;
    private Action startCallback;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Use XR device primary button (A on right-hand controller).
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            callback?.Invoke(TempoEventType.cut);
            callback?.Invoke(TempoEventType.roll);
            callback?.Invoke(TempoEventType.put);
            callback?.Invoke(TempoEventType.send);
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.B))
        {
            startCallback?.Invoke();
        }
    }

    public void SetCallback(Action<TempoEventType> action)
    {
        callback = action;
    }
    public void SetStartCallback(Action action)
    {
        startCallback = action;
    }
}
