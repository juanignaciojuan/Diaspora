using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC2DCharacter : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [Tooltip("List of dialogue lines this NPC will say, one per press of E.")]
    public List<string> npcDialogues = new List<string>();

    [Header("UI References")]
    public GameObject dialogueUI;       // The dialogue panel
    public TextMeshProUGUI dialogueText; // Text field inside panel
    public GameObject promptUI;          // "Press E" prompt
    public TextMeshProUGUI promptText;

    [Header("Audio")]
    public AudioSource dialogueSound;
    public AudioSource responseSound;

    [Header("Options")]
    public Transform playerCamera; // For billboard effect
    public float promptDistance = 3f;

    private bool playerInRange = false;
    private int currentDialogueIndex = 0;

    private void Start()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false);
        if (promptUI != null) promptUI.SetActive(false);

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        // Always face the player (billboard effect)
        if (playerCamera != null)
            transform.LookAt(playerCamera);

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowNextDialogue();
        }
    }

    private void ShowNextDialogue()
    {
        if (dialogueUI == null || dialogueText == null) return;

        // Show current line
        if (currentDialogueIndex < npcDialogues.Count)
        {
            dialogueText.text = npcDialogues[currentDialogueIndex];

            if (dialogueUI != null) dialogueUI.SetActive(true);
            if (dialogueSound != null) dialogueSound.Play();

            currentDialogueIndex++;
        }
        else
        {
            // End of dialogue → close panel and reset
            dialogueUI.SetActive(false);
            if (responseSound != null) responseSound.Play();
            currentDialogueIndex = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptUI != null)
            {
                promptUI.SetActive(true);
                if (promptText != null)
                    promptText.text = "Presiona E para hablar";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptUI != null) promptUI.SetActive(false);
            if (dialogueUI != null) dialogueUI.SetActive(false);
            currentDialogueIndex = 0;
        }
    }
}
