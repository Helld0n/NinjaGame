using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Сцена для запуска игры")]
    public string firstLevelScene = "MainTown";

    // Кнопка «Играть»
    public void PlayGame()
    {
        Debug.Log("Загружаем сцену: " + firstLevelScene);
        SceneManager.LoadScene(firstLevelScene);
    }

    // Кнопка «Выход»
    public void QuitGame()
    {
        Debug.Log("Выход из игры");

        // Закрываем приложение
        Application.Quit();

        // В редакторе Unity — выключаем Play Mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}