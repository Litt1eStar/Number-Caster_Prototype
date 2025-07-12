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
        SceneManager.LoadScene(playMenuSceneName);
    }

    public void NavigateToProfile()
    {

    }
    public void NavigateToShop()
    {

    }

    public void NavigateToDeck()
    {

    }
}
