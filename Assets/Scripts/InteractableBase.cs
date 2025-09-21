using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    public enum InteractionMode { LeftClick, EKey, Both }

    [Header("Interactable Settings")]
    public string hoverMessage = "Interact";
    public bool isLocked = false;
    public InteractionMode interactionMode = InteractionMode.LeftClick;

    protected bool playerInRange = false;
    protected bool isHovering = false;

    // Called when player triggers the interaction input for this object
    public abstract void Interact();

    // Called by PlayerInteractor when we should show a hover hint
    public virtual void ShowHover()
    {
        if (UIManager.instance == null) return;
        UIManager.instance.ShowInteractHint(hoverMessage);
        isHovering = true;
    }

    public virtual void HideHover()
    {
        UIManager.instance?.HideInteractHint();
        isHovering = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideHover();
        }
    }
}
