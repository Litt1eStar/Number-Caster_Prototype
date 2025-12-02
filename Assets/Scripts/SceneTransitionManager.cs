using DG.Tweening;
using static System.TimeZoneInfo;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
public enum TransitionType
{
    Fade,
    SlideLeft,
    SlideRight,
    SlideUp,
    SlideDown,
    Scale,
    CircleWipe
}

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Transition Settings")]
    public GameObject transitionPanel;
    public float transitionDuration = 1f;
    public Ease transitionEase = Ease.InOutQuad;
    public TransitionType defaultTransitionType = TransitionType.Fade;

    private CanvasGroup transitionCanvasGroup;
    private RectTransform transitionRectTransform;
    private Vector2 originalPanelSize;
    private Vector2 originalPanelPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Setup transition panel
            if (transitionPanel != null)
            {
                transitionCanvasGroup = transitionPanel.GetComponent<CanvasGroup>();
                if (transitionCanvasGroup == null)
                {
                    transitionCanvasGroup = transitionPanel.AddComponent<CanvasGroup>();
                }

                transitionRectTransform = transitionPanel.GetComponent<RectTransform>();

                // Store original values
                originalPanelSize = transitionRectTransform.sizeDelta;
                originalPanelPosition = transitionRectTransform.anchoredPosition;

                // Start invisible
                transitionCanvasGroup.alpha = 0f;
                transitionPanel.SetActive(true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use default transition type
    public void TransitionToScene(string sceneName)
    {
        TransitionToScene(sceneName, defaultTransitionType);
    }

    // Use specific transition type
    public void TransitionToScene(string sceneName, TransitionType transitionType)
    {
        StartCoroutine(TransitionCoroutine(sceneName, transitionType));
    }

    private IEnumerator TransitionCoroutine(string sceneName, TransitionType transitionType)
    {
        // Reset panel to initial state
        ResetPanel();

        // Transition IN
        yield return StartCoroutine(TransitionIn(transitionType));

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Wait a frame for scene to load
        yield return new WaitForEndOfFrame();

        // Transition OUT
        yield return StartCoroutine(TransitionOut(transitionType));
    }

    private IEnumerator TransitionIn(TransitionType transitionType)
    {
        switch (transitionType)
        {
            case TransitionType.Fade:
                yield return StartCoroutine(FadeIn());
                break;
            case TransitionType.SlideLeft:
                yield return StartCoroutine(SlideIn(Vector2.right));
                break;
            case TransitionType.SlideRight:
                yield return StartCoroutine(SlideIn(Vector2.left));
                break;
            case TransitionType.SlideUp:
                yield return StartCoroutine(SlideIn(Vector2.down));
                break;
            case TransitionType.SlideDown:
                yield return StartCoroutine(SlideIn(Vector2.up));
                break;
            case TransitionType.Scale:
                yield return StartCoroutine(ScaleIn());
                break;
            case TransitionType.CircleWipe:
                yield return StartCoroutine(CircleWipeIn());
                break;
        }
    }

    private IEnumerator TransitionOut(TransitionType transitionType)
    {
        switch (transitionType)
        {
            case TransitionType.Fade:
                yield return StartCoroutine(FadeOut());
                break;
            case TransitionType.SlideLeft:
                yield return StartCoroutine(SlideOut(Vector2.left));
                break;
            case TransitionType.SlideRight:
                yield return StartCoroutine(SlideOut(Vector2.right));
                break;
            case TransitionType.SlideUp:
                yield return StartCoroutine(SlideOut(Vector2.up));
                break;
            case TransitionType.SlideDown:
                yield return StartCoroutine(SlideOut(Vector2.down));
                break;
            case TransitionType.Scale:
                yield return StartCoroutine(ScaleOut());
                break;
            case TransitionType.CircleWipe:
                yield return StartCoroutine(CircleWipeOut());
                break;
        }
    }

    private void ResetPanel()
    {
        transitionCanvasGroup.alpha = 0f;
        transitionRectTransform.anchoredPosition = originalPanelPosition;
        transitionRectTransform.sizeDelta = originalPanelSize;
        transitionRectTransform.localScale = Vector3.one;
    }

    // Fade Transitions
    private IEnumerator FadeIn()
    {
        transitionCanvasGroup.alpha = 0f;
        transitionCanvasGroup.DOFade(1f, transitionDuration).SetEase(transitionEase);
        yield return new WaitForSeconds(transitionDuration);
    }

    private IEnumerator FadeOut()
    {
        transitionCanvasGroup.DOFade(0f, transitionDuration).SetEase(transitionEase);
        yield return new WaitForSeconds(transitionDuration);
    }

    // Slide Transitions
    private IEnumerator SlideIn(Vector2 direction)
    {
        transitionCanvasGroup.alpha = 1f;

        // Start position (off screen)
        Vector2 startPos = originalPanelPosition + direction * Screen.width;
        transitionRectTransform.anchoredPosition = startPos;

        // Slide to center
        transitionRectTransform.DOAnchorPos(originalPanelPosition, transitionDuration)
            .SetEase(transitionEase);
        yield return new WaitForSeconds(transitionDuration);
    }

    private IEnumerator SlideOut(Vector2 direction)
    {
        // Slide off screen
        Vector2 endPos = originalPanelPosition + direction * Screen.width;
        transitionRectTransform.DOAnchorPos(endPos, transitionDuration)
            .SetEase(transitionEase);
        yield return new WaitForSeconds(transitionDuration);
    }

    // Scale Transitions
    private IEnumerator ScaleIn()
    {
        transitionCanvasGroup.alpha = 1f;
        transitionRectTransform.localScale = Vector3.zero;

        transitionRectTransform.DOScale(Vector3.one, transitionDuration)
            .SetEase(Ease.OutBack);
        yield return new WaitForSeconds(transitionDuration);
    }

    private IEnumerator ScaleOut()
    {
        transitionRectTransform.DOScale(Vector3.zero, transitionDuration)
            .SetEase(Ease.InBack);
        yield return new WaitForSeconds(transitionDuration);
    }

    // Circle Wipe Transitions (using scale with circular mask)
    private IEnumerator CircleWipeIn()
    {
        transitionCanvasGroup.alpha = 1f;
        transitionRectTransform.localScale = Vector3.zero;

        transitionRectTransform.DOScale(Vector3.one, transitionDuration)
            .SetEase(Ease.OutCirc);
        yield return new WaitForSeconds(transitionDuration);
    }

    private IEnumerator CircleWipeOut()
    {
        transitionRectTransform.DOScale(Vector3.zero, transitionDuration)
            .SetEase(Ease.InCirc);
        yield return new WaitForSeconds(transitionDuration);
    }
}