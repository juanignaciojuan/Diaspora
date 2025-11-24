using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Toggles a controls menu using the new Input System.
/// </summary>
public class XRControlsMenu : MonoBehaviour
{
    [Tooltip("The UI GameObject (Canvas or Panel) to toggle.")]
    public GameObject controlsMenuRoot;

    [Tooltip("Input Action to toggle the menu (e.g. Menu button).")]
    public InputActionProperty toggleAction;

    private void OnEnable()
    {
        // Use Started/Canceled for Hold-to-Activate
        toggleAction.action.started += OnPress;
        toggleAction.action.canceled += OnRelease;
        toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        toggleAction.action.started -= OnPress;
        toggleAction.action.canceled -= OnRelease;
        toggleAction.action.Disable();
    }

    private void OnPress(InputAction.CallbackContext ctx)
    {
        SetMenu(true);
    }

    private void OnRelease(InputAction.CallbackContext ctx)
    {
        SetMenu(false);
    }

    public void SetMenu(bool active)
    {
        if (controlsMenuRoot == null) return;

        controlsMenuRoot.SetActive(active);
        
        // Optional: Position the menu in front of the player when opened
        if (active && Camera.main != null)
        {
            // Place 1.5m in front of camera, at camera height
            Transform cam = Camera.main.transform;
            controlsMenuRoot.transform.position = cam.position + cam.forward * 1.5f;
            // Face the camera (UI usually looks back at camera, so look at camera + 180 or LookRotation(forward))
            controlsMenuRoot.transform.rotation = Quaternion.LookRotation(controlsMenuRoot.transform.position - cam.position);
        }
    }
}
