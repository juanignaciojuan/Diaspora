using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;

    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        CheckInteraction();
    }

    private void CheckInteraction()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            InteractableBase interactable = hit.collider.GetComponent<InteractableBase>();
            if (interactable != null)
            {
                interactable.ShowHoverMessage();

                if (interactable is DoorInteraction && Input.GetMouseButtonDown(0))
                {
                    interactable.Interact();
                }

                if (interactable is NPCInteraction && Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }

                return; // Only interact with the first valid object
            }
        }

        // Hide all hints if nothing detected
        HideAllHoverHints();
    }

    private void HideAllHoverHints()
    {
        foreach (var i in FindObjectsOfType<InteractableBase>())
        {
            i.HideHoverMessage();
        }
    }
}
