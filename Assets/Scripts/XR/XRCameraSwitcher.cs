using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Switches between the XR Rig's main camera and a target camera via input action.
/// Disables/enables the appropriate Camera components and AudioListeners.
/// </summary>
public class XRCameraSwitcher : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The XR Rig's main camera (usually tagged MainCamera).")]
    public Camera xrMainCamera;

    [Tooltip("The camera to switch to (e.g. a cutscene or security camera).")]
    public Camera targetCamera;

    [Header("Input")]
    [Tooltip("Input Action to trigger the camera switch (hold or toggle).")]
    public InputActionReference switchCameraAction;

    private bool isSwitched = false;

    private void OnEnable()
    {
        if (switchCameraAction != null)
        {
            switchCameraAction.action.started += OnSwitchStart;
            switchCameraAction.action.canceled += OnSwitchEnd;
            switchCameraAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (switchCameraAction != null)
        {
            switchCameraAction.action.started -= OnSwitchStart;
            switchCameraAction.action.canceled -= OnSwitchEnd;
            switchCameraAction.action.Disable();
        }
        // Always restore to XR camera on disable
        if (isSwitched)
            SwitchToXR();
    }

    private void OnSwitchStart(InputAction.CallbackContext context)
    {
        if (!isSwitched) SwitchToTarget();
    }

    private void OnSwitchEnd(InputAction.CallbackContext context)
    {
        if (isSwitched) SwitchToXR();
    }

    private void SwitchToTarget()
    {
        if (xrMainCamera != null) xrMainCamera.enabled = false;
        if (targetCamera != null) targetCamera.enabled = true;
        // AudioListener: Only one should be enabled
        var xrListener = xrMainCamera != null ? xrMainCamera.GetComponent<AudioListener>() : null;
        var targetListener = targetCamera != null ? targetCamera.GetComponent<AudioListener>() : null;
        if (xrListener != null) xrListener.enabled = false;
        if (targetListener != null) targetListener.enabled = true;
        isSwitched = true;
    }

    private void SwitchToXR()
    {
        if (xrMainCamera != null) xrMainCamera.enabled = true;
        if (targetCamera != null) targetCamera.enabled = false;
        // AudioListener: Only one should be enabled
        var xrListener = xrMainCamera != null ? xrMainCamera.GetComponent<AudioListener>() : null;
        var targetListener = targetCamera != null ? targetCamera.GetComponent<AudioListener>() : null;
        if (xrListener != null) xrListener.enabled = true;
        if (targetListener != null) targetListener.enabled = false;
        isSwitched = false;
    }
}
