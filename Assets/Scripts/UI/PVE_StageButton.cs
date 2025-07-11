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

    public void OnPointerClick(PointerEventData eventData)
    {
        t_stageDetail.gameObject.SetActive(true);
        t_stageDetail.GetComponent<Image>().sprite = stageSprite;
    }
}
