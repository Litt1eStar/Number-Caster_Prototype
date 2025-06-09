using DG.Tweening;
using TMPro;
using UnityEngine;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager Instance { get { return _instance; } }
    private static ErrorManager _instance;

    [SerializeField] private TextMeshProUGUI t_error;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    private string errorMessage = string.Empty;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Start()
    {
        textCanvasGroup.alpha = 0f;
    }
    public void SetErrorMessage(string errMsg)
    {
        errorMessage = errMsg;
        t_error.text = errorMessage;
        AnimateText();
    }

    private void AnimateText()
    {
        Sequence resultSequence = DOTween.Sequence();
        resultSequence.Append(textCanvasGroup.DOFade(1f, fadeDuration)).SetEase(Ease.OutFlash);
        resultSequence.AppendInterval(1f);
        resultSequence.Append(textCanvasGroup.DOFade(0f, fadeDuration)).SetEase(Ease.InFlash);
    }
}