using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RestartOnClick : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float interactionDistance = 5f;

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (playerCamera == null) return;

        // Lanzamos un raycast desde la cámara
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Debug.Log("[RestartOnClick] Black box clicked!");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }
}
