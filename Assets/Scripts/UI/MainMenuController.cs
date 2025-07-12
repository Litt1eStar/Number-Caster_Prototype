using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string playMenuSceneName = "PlayMenu";
    [SerializeField] private string profileSceneName = "Profile";
    [SerializeField] private string shopSceneName = "Shop";
    [SerializeField] private string deckSceneName = "Deck";
    public void NavigateToPlayMenu()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
        SceneManager.LoadScene(playMenuSceneName);
    }

    public void NavigateToProfile()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
    }
    public void NavigateToShop()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
    }

    public void NavigateToDeck()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
    }
}
