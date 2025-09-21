using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class GameStartManager : MonoBehaviour
{
    public Button startButton;
    public GameObject titleTextObject;
    public GameObject instructionsTextObject;
    public FirstPersonController playerController;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
            playerController.enabled = false;

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }

    void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked!");
        
        startButton.gameObject.SetActive(false);
        titleTextObject?.SetActive(false);
        instructionsTextObject?.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
            playerController.enabled = true;
    }
}
