using UnityEngine;

public class NPCDialogueZone : MonoBehaviour
{
    public GameObject dialogueUI;
    public InteractPrompt prompt;
    private bool playerInZone = false;

    void Start()
    {
        if (prompt != null) prompt.Hide();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            if (prompt != null) prompt.Show();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (prompt != null) prompt.Hide();
            if (dialogueUI != null) dialogueUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.StartDialogue("Приветствую, путник! Поможешь мне с одним делом?");
                if (prompt != null) prompt.Hide();
            }
        }
    }
}