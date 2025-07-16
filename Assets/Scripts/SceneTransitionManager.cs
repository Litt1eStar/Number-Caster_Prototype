using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Transition Settings")]
    public GameObject transitionPanel;
    public float transitionDuration = 3f;
    public Ease transitionEase = Ease.InOutQuad;

    private CanvasGroup transitionCanvasGroup;

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

                transitionCanvasGroup.alpha = 0f;
                transitionPanel.SetActive(true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        // Fade to black
        if (transitionCanvasGroup != null)
        {
            transitionCanvasGroup.DOFade(1f, transitionDuration).SetEase(transitionEase);
            yield return new WaitForSeconds(transitionDuration);
        }

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Fade from black (in the new scene)
        yield return new WaitForSeconds(0.1f);
        if (transitionCanvasGroup != null)
        {
            transitionCanvasGroup.DOFade(0f, transitionDuration).SetEase(transitionEase);
        }
    }

}
