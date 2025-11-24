using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Restarts the current scene when a specific button is pressed.
/// </summary>
public class XRGameRestarter : MonoBehaviour
{
    [Tooltip("Input Action to restart the game (e.g. Left Controller Menu Button).")]
    public InputActionProperty restartAction;

    private void OnEnable()
    {
        if (restartAction.action != null)
        {
            restartAction.action.performed += OnRestart;
            restartAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (restartAction.action != null)
        {
            restartAction.action.performed -= OnRestart;
            restartAction.action.Disable();
        }
    }

    private void OnRestart(InputAction.CallbackContext ctx)
    {
        Debug.Log("Restarting Scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
