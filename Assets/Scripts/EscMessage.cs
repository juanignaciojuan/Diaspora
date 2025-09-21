using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class EscMessageDualInputConfigurable : MonoBehaviour
{
    [Header("UI & Sound")]
    public string messageToShow = "ESC pressed!";
    public AudioSource audioSource;    
    public float displayTime = 3f;

    [Header("New Input System (optional)")]
    public InputActionReference escAction;

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
        // Always check the old Input for ESC
        if (Input.GetKeyDown(KeyCode.Escape) && !isShowing)
            TriggerMessage();
    }

    private void OnEscPressed(InputAction.CallbackContext context)
    {
        if (!isShowing)
            TriggerMessage();
    }

    private void TriggerMessage()
    {
        if (UIManager.instance != null)
            UIManager.instance.ShowMessage(messageToShow);

        if (audioSource != null)
            audioSource.Play();

        StartCoroutine(MessageCooldown());
    }

    private IEnumerator MessageCooldown()
    {
        isShowing = true;
        yield return new WaitForSeconds(displayTime);
        isShowing = false;
    }
}
