using UnityEngine;

public class NPCDialogueZone : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogueUI;
    public InteractPrompt prompt;

    [Header("Данные NPC")]
    public string npcName = "NPC";
    [TextArea(3, 6)]
    public string dialogueText = "Приветствую, путник!";
    public string questSceneName = "QuestScene";   // Имя сцены, куда перебросит

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
                DialogueManager.instance.StartDialogue(dialogueText, npcName, questSceneName);
                if (prompt != null) prompt.Hide();
            }
        }
    }
}