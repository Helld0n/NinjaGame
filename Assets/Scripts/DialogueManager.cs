using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Элементы")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcText;
    public GameObject choicePanel;

    [Header("Кнопки выбора")]
    public Button acceptButton;
    public Button declineButton;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string npcDialogue)
    {
        dialoguePanel.SetActive(true);
        npcText.text = npcDialogue;
        choicePanel.SetActive(true);
    }

    public void OnAcceptQuest()
    {
        UnityEngine.Debug.Log("Квест принят! Загружаем сцену...");
        SceneManager.LoadScene("QuestScene");
    }

    public void OnDeclineQuest()
    {
        UnityEngine.Debug.Log("Квест отклонён.");
        dialoguePanel.SetActive(false);
    }
}