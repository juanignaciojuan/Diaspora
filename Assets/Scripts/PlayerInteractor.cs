using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast")]
    public float interactDistance = 3f;
    public LayerMask interactLayer = ~0;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerDialogueManager dialogueManager;

    private InteractableBase currentInteractable;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (dialogueManager == null)
            dialogueManager = Object.FindFirstObjectByType<PlayerDialogueManager>();

        if (playerCamera == null)
            Debug.LogError("PlayerInteractor: No camera assigned!");
    }

    private void Update()
    {
        DoHoverRaycast();
        HandleInput();
        HandleRandomTalk();
    }

    private void DoHoverRaycast()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer, QueryTriggerInteraction.Collide))
        {
            var hitInteract = hit.collider.GetComponentInParent<InteractableBase>();
            if (hitInteract != null)
            {
                if (currentInteractable != hitInteract)
                {
                    if (currentInteractable != null) currentInteractable.HideHover();
                    currentInteractable = hitInteract;
                    currentInteractable.ShowHover();
                }
                return;
            }
        }

        // nothing hit
        if (currentInteractable != null)
        {
            currentInteractable.HideHover();
            currentInteractable = null;
        }
    }

    private void HandleInput()
    {
        if (currentInteractable == null) return;

        // Left click
        if (Input.GetMouseButtonDown(0) &&
            (currentInteractable.interactionMode == InteractableBase.InteractionMode.LeftClick ||
             currentInteractable.interactionMode == InteractableBase.InteractionMode.Both))
        {
            currentInteractable.Interact();
        }

        // E key
        if (Input.GetKeyDown(KeyCode.E) &&
            (currentInteractable.interactionMode == InteractableBase.InteractionMode.EKey ||
             currentInteractable.interactionMode == InteractableBase.InteractionMode.Both))
        {
            currentInteractable.Interact();
        }
    }

    private void HandleRandomTalk()
    {
        if (dialogueManager == null) return;
        if (Input.GetKeyDown(KeyCode.T))
            dialogueManager.PlayRandomDialogue();
    }
}
