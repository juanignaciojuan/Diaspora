using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [Header("Hover Hint")]
    public string hoverMessage = "Press E to interact";

    public virtual void ShowHoverMessage()
    {
        UIManager.instance?.ShowInteractHint(hoverMessage);
    }

    public virtual void HideHoverMessage()
    {
        UIManager.instance?.HideInteractHint();
    }

    // Called when the player interacts with this object
    public abstract void Interact();
}
