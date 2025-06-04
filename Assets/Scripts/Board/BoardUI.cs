using DG.Tweening;
using TMPro;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private GameObject btnContainer;
    [SerializeField] private GameObject resultTextContainer;
    [SerializeField] private CanvasGroup resultCanvasGroup;
    [SerializeField] private TextMeshProUGUI t_rawValue;
    [SerializeField] private TextMeshProUGUI t_cappedValue;

    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float moveDistance = 500f;
    [SerializeField] private float shownDuration = 1.5f;
    private Vector3 originalPosition;
    private Vector3 hiddenPosition;
    private void Start()
    {
        btnContainer.SetActive(false);

        originalPosition = resultTextContainer.transform.localPosition;
        hiddenPosition = new Vector3(originalPosition.x, originalPosition.y + moveDistance, originalPosition.z);

        resultTextContainer.transform.localPosition = hiddenPosition;
        resultTextContainer.SetActive(false);

        if (resultCanvasGroup == null)
        {
            resultCanvasGroup = resultTextContainer.GetComponent<CanvasGroup>();
            if (resultCanvasGroup == null)
            {
                resultCanvasGroup = resultTextContainer.AddComponent<CanvasGroup>();
            }
        }
        resultCanvasGroup.alpha = 0f;
    }

    public void ShowButton()
    {
        btnContainer.SetActive(true);
    }

    public void HideButton()
    {
        btnContainer.SetActive(false);
    }

    public void ShowResult(int rawValue, int cappedValue)
    {
        t_rawValue.text = $"Raw Value: {rawValue}";
        t_cappedValue.text = $"Capped Value: {cappedValue}";
        resultTextContainer.SetActive(true);

        resultTextContainer.transform.localPosition = hiddenPosition;
        resultCanvasGroup.alpha = 0f;

        Sequence resultSequence = DOTween.Sequence();

        resultSequence.Append(resultTextContainer.transform.DOLocalMove(originalPosition, fadeDuration).SetEase(Ease.OutBounce));
        resultSequence.Join(resultCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad));

        resultSequence.AppendInterval(shownDuration);

        resultSequence.Append(resultTextContainer.transform.DOLocalMove(hiddenPosition, fadeDuration * 0.7f).SetEase(Ease.InQuad));
        resultSequence.Join(resultCanvasGroup.DOFade(0f, fadeDuration * 0.7f).SetEase(Ease.InQuad));

        resultSequence.OnComplete(() => resultTextContainer.SetActive(false));
    }
}
