using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum StageLevel
{
    Monster,
    MiniBoss,
    Boss,
}
public class PVE_StageButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ClassSO enemyClass;
    [SerializeField] private bool isClear = false;
    [SerializeField] private StageLevel stageLevel;
    [SerializeField] private Sprite stageSprite;
    [SerializeField] private Transform t_stageDetail;
    [SerializeField] private Sprite gameplayBackgroundSprite;

    public float hoverScale = 1.3f;
    public float duration = 0.2f;

    private PVE_Map_Controller mapController;

    private void Start()
    {
        mapController = FindFirstObjectByType<PVE_Map_Controller>();
        if (mapController == null)
        {
            Debug.LogError("PVE_Map_Controller not found in the scene.");
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (mapController.isPanelOpen) return;

        t_stageDetail.GetComponent<Image>().sprite = stageSprite;
        t_stageDetail.DOScale(transform.localScale * hoverScale, duration).SetEase(Ease.OutBack).OnComplete(() => mapController.isPanelOpen = true);
        mapController.SetButtonInteractable(false);
        
        mapController.enemyClass = enemyClass;
        mapController.gameplayBackgroundSprite = gameplayBackgroundSprite;
    }
}
