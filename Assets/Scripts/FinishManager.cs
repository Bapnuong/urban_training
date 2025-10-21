using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishManager : MonoBehaviour
{
    public Button buttonMenu;
    public Button buttonRetry;

    void Start()
    {
        if (buttonMenu != null)
            buttonMenu.onClick.AddListener(BackToMenu);

        if (buttonRetry != null)
            buttonRetry.onClick.AddListener(RestartLevel);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("gamescene");
    }
}
