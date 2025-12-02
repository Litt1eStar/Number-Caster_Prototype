using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuController : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    public void NavigateToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
        AudioManager.Instance.PlaySFX("Return-Btn");
    }
}
