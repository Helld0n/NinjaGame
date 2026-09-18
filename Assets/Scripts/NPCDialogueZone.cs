using UnityEngine;

public class NPCDialogueZone : MonoBehaviour
{
    public GameObject dialogueUI;
    private bool playerInZone = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
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
            }
        }
    }
}