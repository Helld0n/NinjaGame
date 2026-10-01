using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Настройки уровня")]
    public int requiredKills = 15;
    public int currentKills = 0;

    [Header("UI — счётчик")]
    public TextMeshProUGUI killCounterText;

    [Header("UI — экран победы")]
    public GameObject victoryScreen;

    [Header("Сцены")]
    public string townSceneName = "MainTown";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (victoryScreen != null) victoryScreen.SetActive(false);
        UpdateCounterUI();
    }

    public void AddKill()
    {
        currentKills++;
        UpdateCounterUI();

        if (currentKills >= requiredKills)
        {
            LevelComplete();
        }
    }

    void UpdateCounterUI()
    {
        if (killCounterText != null)
        {
            killCounterText.text = "Убито: " + currentKills + " / " + requiredKills;
        }
    }

    void LevelComplete()
    {
        Debug.Log("Уровень пройден!");

        if (victoryScreen != null) victoryScreen.SetActive(true);
    }

    public void ReturnToTown()
    {
        SceneManager.LoadScene(townSceneName);
    }
}