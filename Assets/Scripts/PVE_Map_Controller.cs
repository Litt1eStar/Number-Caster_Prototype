using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PVE_Map_Controller : MonoBehaviour
{
    public ClassSO enemyClass;
    public ClassSO playerClass;
    public Transform t_stageDetail;
    public Sprite gameplayBackgroundSprite;

    public float hoverScale = 1.3f;
    public float duration = 0.2f;

    public bool isPanelOpen = false;
    [SerializeField] private string prevSceneName = "PlayMenu";
    private bool isTransitioning = false;
    private Vector3 originalScale;

    private void Start()
    {
        if (t_stageDetail == null)
        {
            Debug.LogError("Stage detail panel not assigned in the inspector.");
        }

        originalScale = transform.localScale;
    }
    public void StartMatch()
    {
        DataPersistance.Instance.enemyClass = enemyClass;
        DataPersistance.Instance.playerClass = playerClass;
        DataPersistance.Instance.gameplayBackgroundSprite = gameplayBackgroundSprite;

        AudioManager.Instance.PlaySFX("Button-Click");

        AudioManager.Instance.PlaySFX("Button-Click");
        if (!isTransitioning && SceneTransitionManager.Instance != null)
        {
            isTransitioning = true;
            transform.DOScale(originalScale * 0.9f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad);
                    SceneTransitionManager.Instance.TransitionToScene("Gameplay", TransitionType.Fade);
                });

        }
    }
    public void NavigateToPrevScene()
    {
        SceneManager.LoadScene(prevSceneName);
        AudioManager.Instance.PlaySFX("Return-Btn");
    }
    public void CloseStageDetailPanel()
    {
        t_stageDetail.DOScale(Vector3.zero * hoverScale, duration).SetEase(Ease.InOutSine).OnComplete(() => isPanelOpen = false);
        SetButtonInteractable(true);
        AudioManager.Instance.PlaySFX("Close-Btn");
    }

    public void SetButtonInteractable(bool isInteractable)
    {
        foreach (var button in FindObjectsOfType<PVE_StageButton>())
        {
            button.GetComponent<Button>().interactable = isInteractable;
        }
    }
}
