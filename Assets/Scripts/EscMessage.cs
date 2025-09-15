using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // For the new Input System
using System.Collections;

public class EscMessageDualInputConfigurable : MonoBehaviour
{
    [Header("UI & Sound")]
    public TMP_Text messageText;       // Assign your Canvas → MessageText here
    public AudioSource audioSource;    // Sound to play
    public float displayTime = 3f;     // Duration of message
    [TextArea] public string messageToShow = "Press ESC detected!"; // Customizable text

    [Header("Old Input System")]
    public KeyCode testKey = KeyCode.F; // Configurable key in Inspector (default = F)

    [Header("New Input System")]
    public InputActionReference escAction; // Optional: assign an Input Action if using the new system

    private bool isShowing = false;

    private void OnEnable()
    {
        if (escAction != null)
            escAction.action.performed += OnEscPressed;
    }

    private void OnDisable()
    {
        if (escAction != null)
            escAction.action.performed -= OnEscPressed;
    }

    private void Update()
    {
        // Old Input System fallback
        if (Input.GetKeyDown(testKey) && !isShowing)
        {
            TriggerMessage();
        }
    }

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
            messageText.text = messageToShow; // Update text instead of hiding/showing

        yield return new WaitForSeconds(displayTime);

        if (messageText != null)
            messageText.text = ""; // Clear instead of disabling

        isShowing = false;
    }
}
