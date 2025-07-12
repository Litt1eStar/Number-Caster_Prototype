using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string destinationSceneName = "PVE_MAP";
    public float hoverScale = 1.3f;
    public float duration = 0.2f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * hoverScale, duration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration).SetEase(Ease.InOutSine);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(destinationSceneName);
    }
}
