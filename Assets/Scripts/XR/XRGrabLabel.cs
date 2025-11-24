using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Shows a label (GameObject) when this object is grabbed, and hides it when released.
/// </summary>
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class XRGrabLabel : MonoBehaviour
{
    [Tooltip("The label object (e.g. Canvas/Text) to show when grabbed.")]
    public GameObject labelObject;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (labelObject != null)
            labelObject.SetActive(false);
    }

    private void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(OnGrab);
        _grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrab);
        _grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (labelObject != null)
            labelObject.SetActive(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (labelObject != null)
            labelObject.SetActive(false);
    }
}
