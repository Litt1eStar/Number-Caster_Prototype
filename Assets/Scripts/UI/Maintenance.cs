using DG.Tweening;
using UnityEngine;

public class Maintenance : MonoBehaviour
{
    private bool isTransitioning = false;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }
    public void NavigateToMainmenu()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
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
