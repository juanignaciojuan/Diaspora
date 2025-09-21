using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class EscMessageDualInputConfigurable : MonoBehaviour
{
    [Header("UI & Sound")]
    public string messageToShow = "Press ESC detected!";
    public AudioSource audioSource;    
    public float displayTime = 3f;

    [Header("Old Input System")]
    public KeyCode testKey = KeyCode.F;

    [Header("New Input System")]
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
        if (Input.GetKeyDown(testKey) && !isShowing)
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
