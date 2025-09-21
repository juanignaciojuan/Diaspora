using UnityEngine;
using TMPro;

public class NPCInteractable : InteractableBase
{
    [Header("Dialogue Settings")]
    public string[] dialogueLines;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    private int currentIndex = 0;
    private bool dialogueActive = false;

    private void Awake()
    {
        // NPCs use E key
        interactionMode = InteractionMode.EKey;
    }

    public override void Interact()
    {
        if (dialoguePanel == null || dialogueText == null) return;

        if (!dialogueActive)
        {
            // first time you press E
            dialogueActive = true;
            dialoguePanel.SetActive(true);
            ShowNextLine();
        }
        else
        {
            // every other press of E continues the dialogue
            ShowNextLine();
        }
    }

    private void ShowNextLine()
    {
        if (currentIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentIndex];
            currentIndex++;
        }
        else
        {
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueActive = false;
        currentIndex = 0;
    }

    public override void ShowHover()
    {
        UIManager.instance?.ShowInteractHint("Press E to talk");
        isHovering = true;
    }
}
