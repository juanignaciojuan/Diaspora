using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class GrabLabelBinder : MonoBehaviour
{
    public enum InteractionMode
    {
        GrabOnly,
        HoverOnly,
        Both
    }

    [Tooltip("When should the label appear?")]
    public InteractionMode showMode = InteractionMode.GrabOnly;

    [Tooltip("Text to display when this object is grabbed or hovered")]
    public string labelText = "Grabbed";

    [Tooltip("Offset applied to the label position (world space)")]
    public Vector3 labelOffset = new Vector3(0f, 0.2f, 0f);

    [Tooltip("Other interactables that, if grabbed, should prevent this label from showing on hover.")]
    public System.Collections.Generic.List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable> suppressIfGrabbed;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;

    void Awake()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnEnable()
    {
        _grab.selectEntered.AddListener(OnGrab);
        _grab.selectExited.AddListener(OnRelease);
        _grab.hoverEntered.AddListener(OnHoverEnter);
        _grab.hoverExited.AddListener(OnHoverExit);
    }

    void OnDisable()
    {
        _grab.selectEntered.RemoveListener(OnGrab);
        _grab.selectExited.RemoveListener(OnRelease);
        _grab.hoverEntered.RemoveListener(OnHoverEnter);
        _grab.hoverExited.RemoveListener(OnHoverExit);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (showMode == InteractionMode.GrabOnly || showMode == InteractionMode.Both)
        {
            ShowLabel();
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // If we are in Both mode, and we are still hovering, don't hide.
        if (showMode == InteractionMode.Both && _grab.isHovered)
        {
            return;
        }

        if (showMode == InteractionMode.GrabOnly || showMode == InteractionMode.Both)
        {
            HideLabel();
        }
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        // Check if any linked interactable is currently grabbed
        if (suppressIfGrabbed != null)
        {
            foreach (var linked in suppressIfGrabbed)
            {
                if (linked != null && linked.isSelected) return;
            }
        }

        if (showMode == InteractionMode.HoverOnly || showMode == InteractionMode.Both)
        {
            ShowLabel();
        }
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        // If we are in Both mode, and we are still selected (grabbed), don't hide.
        if (showMode == InteractionMode.Both && _grab.isSelected)
        {
            return;
        }

        if (showMode == InteractionMode.HoverOnly || showMode == InteractionMode.Both)
        {
            HideLabel();
        }
    }

    private void ShowLabel()
    {
        if (GrabLabelManager.Instance != null)
            GrabLabelManager.Instance.Show(transform, labelText, labelOffset);
    }

    private void HideLabel()
    {
        if (GrabLabelManager.Instance != null)
            GrabLabelManager.Instance.Hide(transform);
    }
}
