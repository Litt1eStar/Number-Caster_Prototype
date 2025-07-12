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

    private void Start()
    {
        if (t_stageDetail == null)
        {
            Debug.LogError("Stage detail panel not assigned in the inspector.");
        }
    }
    public void StartMatch()
    {
        DataPersistance.Instance.enemyClass = enemyClass;
        DataPersistance.Instance.playerClass = playerClass;
        DataPersistance.Instance.gameplayBackgroundSprite = gameplayBackgroundSprite;

        SceneManager.LoadScene("Gameplay");
    }

    public void CloseStageDetailPanel()
    {
        t_stageDetail.DOScale(Vector3.zero * hoverScale, duration).SetEase(Ease.InOutSine).OnComplete(() => isPanelOpen = false);
        SetButtonInteractable(true);
    }

    public void SetButtonInteractable(bool isInteractable)
    {
        foreach (var button in FindObjectsOfType<PVE_StageButton>())
        {
            button.GetComponent<Button>().interactable = isInteractable;
        }
    }
}
