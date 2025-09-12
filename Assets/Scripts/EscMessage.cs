using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // For the new Input System
using System.Collections;

public class EscMessageDualInputConfigurable : MonoBehaviour
{
    [Header("UI & Sound")]
    public TMP_Text messageText;       // Your pre-made TextMeshPro UI
    public AudioSource audioSource;    // Sound to play
    public float displayTime = 3f;     // Duration of message

    [Header("Old Input System")]
    public KeyCode testKey = KeyCode.F; // Configurable key in Inspector (default = F)

    [Header("New Input System")]
    public InputActionReference escAction; // Optional: assign an Input Action if using the new system

    private bool isShowing = false;

    private void OnEnable()
    {
        // Subscribe to new Input System event if available
        if (escAction != null)
            escAction.action.performed += OnEscPressed;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        if (escAction != null)
            escAction.action.performed -= OnEscPressed;
    }

    private void Update()
    {
        // Old Input System fallback with configurable key
        if (Input.GetKeyDown(testKey) && !isShowing)
        {
            TriggerMessage();
        }
    }

    // Called when the New Input System action is performed
    private void OnEscPressed(InputAction.CallbackContext context)
    {
        if (!isShowing)
            TriggerMessage();
    }

    private void TriggerMessage()
    {
        StartCoroutine(ShowMessage());
        if (audioSource != null)
            audioSource.Play();
    }

    private IEnumerator ShowMessage()
    {
        isShowing = true;

        if (messageText != null)
            messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        if (messageText != null)
            messageText.gameObject.SetActive(false);

        isShowing = false;
    }
}
