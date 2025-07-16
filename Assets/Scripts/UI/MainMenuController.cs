using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string playMenuSceneName = "PlayMenu";
    [SerializeField] private string profileSceneName = "Profile";
    [SerializeField] private string shopSceneName = "Shop";
    [SerializeField] private string deckSceneName = "Deck";

    [SerializeField] private Image profileImage;
    [SerializeField] private TextMeshProUGUI playerNameText;
    private bool isTransitioning = false;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        if (DataPersistance.Instance != null)
        {
            playerNameText.text = DataPersistance.Instance.playerName;
            profileImage.sprite = DataPersistance.Instance.playerProfileSprite;
        }
    }
    public void NavigateToPlayMenu()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
        if (!isTransitioning && SceneTransitionManager.Instance != null)
        {
            isTransitioning = true;
            transform.DOScale(originalScale * 0.9f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad);
                    SceneTransitionManager.Instance.TransitionToScene(playMenuSceneName, TransitionType.SlideLeft);
                });

        }
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
