using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour
{
    public string profileName;
    public Sprite profileSprite;
    public Image profileImage;

    private bool isTransitioning = false;
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void UpdateName(string s)
    {
        profileName = s;
    }
    public void UpdateSprite(Sprite s)
    {
        profileSprite = s;
        profileImage.sprite = profileSprite;
    }

    public void Done()
    {
        DataPersistance.Instance.playerName = profileName;
        DataPersistance.Instance.playerProfileSprite = profileSprite;
        if (!isTransitioning && SceneTransitionManager.Instance != null)
        {
            isTransitioning = true;
            transform.DOScale(originalScale * 0.9f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad);
                    SceneTransitionManager.Instance.TransitionToScene("MainMenu", TransitionType.Fade);
                });

        }
    }

}
