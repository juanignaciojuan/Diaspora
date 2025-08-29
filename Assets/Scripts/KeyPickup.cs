using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private LayerMask keyLayerMask;
    [SerializeField] private AudioSource pickupSound;
    [SerializeField] private DoorInteraction doorToUnlock;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject outlineVisual;

    private bool collected = false;
    private bool isHovered = false;

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        if (outlineVisual != null)
            outlineVisual.SetActive(false);
    }

    private void Update()
    {
        if (collected || playerCamera == null) return;

        bool hitThisFrame = false;

        Debug.DrawRay(playerCamera.position, playerCamera.forward * pickupDistance, Color.red);
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, keyLayerMask))
        {
            Debug.Log($"[KeyPickup] Raycast hit: {hit.collider.name}");

            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("[KeyPickup] Hovering key: " + gameObject.name);
                hitThisFrame = true;

                if (!isHovered)
                {
                    EnableOutline();
                    UIManager.instance.ShowLiveMessage("Tomar la llave");
                    isHovered = true;
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Debug.Log("[KeyPickup] Mouse click detected on key: " + gameObject.name);
                    PickupKey();
                }
            }
        }
        else
        {
            Debug.Log("[KeyPickup] Raycast did not hit anything");
        }

        if (!hitThisFrame && isHovered)
        {
            Debug.Log("[KeyPickup] Stopped hovering key: " + gameObject.name);
            DisableOutline();
            UIManager.instance.ClearMessage();
            isHovered = false;
        }
    }

    private void PickupKey()
    {
        collected = true;
        Debug.Log("[KeyPickup] Key collected: " + gameObject.name);

        GameManager.instance.hasKey = true;

        UIManager.instance.ClearMessage();
        UIManager.instance.ShowMessage("Llave tomada");

        pickupSound?.Play();

        Destroy(gameObject, pickupSound != null ? pickupSound.clip.length : 0.1f);
    }

    public void EnableOutline() => outlineVisual?.SetActive(true);
    public void DisableOutline() => outlineVisual?.SetActive(false);
}
