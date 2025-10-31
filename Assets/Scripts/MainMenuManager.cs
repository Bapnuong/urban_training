using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;               
public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button quitButton;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnOptionsClicked()
    {

    }

    public void OnQuitClicked()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
