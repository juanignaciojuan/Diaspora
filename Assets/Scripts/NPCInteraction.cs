using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class NPCInteraction : InteractableBase
{
    [Header("Dialogue Settings")]
    [TextArea] public string dialogueLine = "Hello, traveler.";
    public string option1Text = "Option 1";
    public string option2Text = "Option 2";

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button option1Button;
    public Button option2Button;
    public TMP_Text option1Label;
    public TMP_Text option2Label;

    [Header("Player Control")]
    public FirstPersonController playerController;

    private bool isActive = false;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public override void Interact()
    {
        ShowDialogue();
    }

    public void ShowDialogue()
    {
        if (dialoguePanel == null) return;

        isActive = true;
        dialoguePanel.SetActive(true);

        dialogueText.text = dialogueLine;
        option1Label.text = option1Text;
        option2Label.text = option2Text;

        // Lock movement & show cursor
        if (playerController != null) playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Buttons
        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        option1Button.onClick.AddListener(() => OptionSelected(option1Text));
        option2Button.onClick.AddListener(() => OptionSelected(option2Text));
    }

    private void OptionSelected(string choice)
    {
        Debug.Log("Player chose: " + choice);
        CloseDialogue();
    }

    private void CloseDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isActive = false;

        if (playerController != null) playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void ShowHoverMessage()
    {
        if (!isActive) base.ShowHoverMessage();
    }
}
