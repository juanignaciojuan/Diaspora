using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets; // For FirstPersonController if you're using it

public class HumanoidInteraction : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public string dialogueLine = "Hello, traveler. What do you want?";
    public string option1Text = "Ask for help";
    public string option2Text = "Walk away";

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button option1Button;
    public Button option2Button;
    public TMP_Text option1Label;
    public TMP_Text option2Label;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip humanoidSpeechClip;   // plays when humanoid talks
    public AudioClip responseClip;         // plays when player responds

    [Header("Player Control")]
    public FirstPersonController playerController; // assign your player here

    private bool isActive = false;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>(); // fallback
    }

    private void OnMouseDown()
    {
        if (!isActive)
            ShowDialogue();
    }

    void ShowDialogue()
    {
        if (dialoguePanel == null) return;

        isActive = true;
        dialoguePanel.SetActive(true);

        dialogueText.text = dialogueLine;
        option1Label.text = option1Text;
        option2Label.text = option2Text;

        // Lock player movement & show cursor
        if (playerController != null)
            playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Play humanoid speech
        if (audioSource != null && humanoidSpeechClip != null)
        {
            audioSource.clip = humanoidSpeechClip;
            audioSource.Play();
        }

        // Reset button listeners
        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();

        option1Button.onClick.AddListener(() => OptionSelected(option1Text));
        option2Button.onClick.AddListener(() => OptionSelected(option2Text));
    }

    void OptionSelected(string choice)
    {
        Debug.Log("Player chose: " + choice);

        // Play response clip
        if (audioSource != null && responseClip != null)
        {
            audioSource.clip = responseClip;
            audioSource.Play();
        }

        CloseDialogue();
    }

    void CloseDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isActive = false;

        // Unlock player movement & hide cursor
        if (playerController != null)
            playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
