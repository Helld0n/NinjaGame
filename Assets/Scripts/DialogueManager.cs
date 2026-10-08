using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Элементы")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcText;
    public GameObject choicePanel;

    [Header("Кнопки")]
    public Button acceptButton;
    public Button declineButton;

    private string currentQuestScene = "QuestScene";

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    // Запуск диалога с параметрами
    public void StartDialogue(string dialogueText, string npcName, string questScene)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (npcText != null) npcText.text = dialogueText;
        if (choicePanel != null) choicePanel.SetActive(true);

        currentQuestScene = questScene;   // Запоминаем, куда перебросить игрока
    }

    public void OnAcceptQuest()
    {
        Debug.Log("Квест принят! Загружаем сцену: " + currentQuestScene);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        SceneManager.LoadScene(currentQuestScene);
    }

    public void OnDeclineQuest()
    {
        Debug.Log("Квест отклонён.");
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}