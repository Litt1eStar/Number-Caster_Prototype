    using DG.Tweening;
    using TMPro;
    using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    [Header("Button Reference")]
    [SerializeField] private GameObject btnContainer;
    [SerializeField] private Button endTurnBtn;

    [Header("Board Reference")]
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private Transform settingContainer;
    [SerializeField] private Transform matchResutlContainer;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color dehighlightColor;
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private TextMeshProUGUI t_playerOnLeaderBoard;
    [SerializeField] private Transform tableCapContainer;

    [Header("Result Text Reference")]
    [SerializeField] private GameObject resultTextContainer;
    [SerializeField] private CanvasGroup resultCanvasGroup;
    [SerializeField] private TextMeshProUGUI t_rawValue;
    [SerializeField] private TextMeshProUGUI t_cappedValue;
    [SerializeField] private TextMeshProUGUI t_timer;

    [Header("Card Detail Reference")]
    [SerializeField] private GameObject cardDetailContainer;
    [SerializeField] private CanvasGroup cardDetailCanvasGroup;
    [SerializeField] private TextMeshProUGUI t_cardName;
    [SerializeField] private TextMeshProUGUI t_cardValue;
    [SerializeField] private TextMeshProUGUI t_cardLevel;
    [SerializeField] private TextMeshProUGUI t_cardDescription;
    [SerializeField] private Image cardImage;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float moveDistance = 500f;
    [SerializeField] private float shownDuration = 1.5f;

    [Header("Player Settings")]
    [SerializeField] private Image img_player;
    [SerializeField] private Image img_playerSkill;
    [SerializeField] private TextMeshProUGUI t_playerClass;
    [SerializeField] private TextMeshProUGUI t_playerHP;
    [SerializeField] private TextMeshProUGUI t_playerArmor;
    [SerializeField] private TextMeshProUGUI t_playerSkill;
    [SerializeField] private Slider slider_playerHP;
    [SerializeField] private Transform playerDeckContainer;
    [SerializeField] private Transform manaContainer;
    [SerializeField] private Image playerHighlight;
    private ClassSO currentPlayerClass;

    [Header("Enemy Settings")]
    [SerializeField] private Image img_enemy;
    [SerializeField] private Image img_enemySkill;
    [SerializeField] private TextMeshProUGUI t_enemyClass;
    [SerializeField] private TextMeshProUGUI t_enemyHP;
    [SerializeField] private TextMeshProUGUI t_enemyArmor;
    [SerializeField] private TextMeshProUGUI t_enemySkill;
    [SerializeField] private TextMeshProUGUI t_enemyMana;
    [SerializeField] private Slider slider_enemyHP;
    [SerializeField] private Transform enemyDeckContainer;
    [SerializeField] private Image enemyHighlight;
    private ClassSO currentEnemyClass;

    [Header("Prefab Setting")]
    [SerializeField] private GameObject manaPrefab;

    public bool onHidingPanel { get; private set; } = false;
    public bool isCardDetailShown { get; private set; } = false;
    private Vector3 result_originalPosition;
    private Vector3 leaderboard_originalPosition;

    private Vector3 hiddenPosition;
    private Vector3 result_originalScale;
    private Vector3 leaderboard_originalScale;

    [SerializeField] private float tableCap_offset = 30f;
    [SerializeField] private Vector3 tableCap_shownPosition;
    private Vector3 tableCap_hiddenPosition;
    private float tableCap_moveDuration = 0.5f;

    private bool isTransitioning = false;

    private void Start()
    {
        tableCap_hiddenPosition = tableCapContainer.transform.localPosition;
        tableCap_shownPosition = tableCap_hiddenPosition + new Vector3(tableCap_offset, 0f, 0f);

        result_originalScale = transform.localScale;
        btnContainer.SetActive(false);

        result_originalPosition = resultTextContainer.transform.localPosition;
        leaderboard_originalPosition = leaderboardContainer.transform.localPosition;
        hiddenPosition = new Vector3(result_originalPosition.x, result_originalPosition.y + moveDistance, result_originalPosition.z);

        resultTextContainer.transform.localPosition = hiddenPosition;
        resultTextContainer.SetActive(false);

        leaderboardContainer.transform.localPosition = hiddenPosition;

        if (resultCanvasGroup == null)
        {
            resultCanvasGroup = resultTextContainer.GetComponent<CanvasGroup>();
            if (resultCanvasGroup == null)
            {
                resultCanvasGroup = resultTextContainer.AddComponent<CanvasGroup>();
            }
        }
        resultCanvasGroup.alpha = 0f;

        if (cardDetailCanvasGroup == null)
        {
            cardDetailCanvasGroup = cardDetailContainer.GetComponent<CanvasGroup>();
            if (cardDetailCanvasGroup == null)
            {
                cardDetailCanvasGroup = cardDetailContainer.AddComponent<CanvasGroup>();
            }
        }
        cardDetailCanvasGroup.alpha = 0f;

        backgroundImage.sprite = DataPersistance.Instance.gameplayBackgroundSprite;
    }
    private void Update()
    {
        if (TurnManager.Instance.currentTurn == Turn.ENEMY)
        {
            //highlight enemy
            //dehighlight player
            enemyHighlight.color = highlightColor;
            playerHighlight.color = dehighlightColor;
            endTurnBtn.interactable = false;
        }
        else if (TurnManager.Instance.currentTurn == Turn.PLAYER)
        {
            //highlight player
            //dehighlight enemy
            playerHighlight.color = highlightColor;
            enemyHighlight.color = dehighlightColor;
            endTurnBtn.interactable = true;
        }
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

        resultSequence.Append(resultTextContainer.transform.DOLocalMove(result_originalPosition, fadeDuration).SetEase(Ease.OutFlash));
        resultSequence.Join(resultCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad));

        resultSequence.AppendInterval(shownDuration);

        resultSequence.Append(resultTextContainer.transform.DOLocalMove(hiddenPosition, fadeDuration * 0.7f).SetEase(Ease.InQuad));
        resultSequence.Join(resultCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InQuad));

        resultSequence.OnComplete(() => resultTextContainer.SetActive(false));
    }
    public void ShowCardDetail(Card shownCard)
    {
        Sequence resultSequence = DOTween.Sequence();
        cardImage.sprite = shownCard.cardData.cardImage;
        t_cardName.text = "Card Name : " + shownCard.cardData.cardName;
        t_cardValue.text = "Card Value : " + shownCard.cardData.cardValue.ToString();
        t_cardLevel.text = "Card Level : " + shownCard.cardData.cardLevel.ToString();
        t_cardDescription.text = "Card Description : " + shownCard.cardData.cardDescription;
        resultSequence.Append(cardDetailCanvasGroup.DOFade(1f, fadeDuration)).SetEase(Ease.OutFlash);

        isCardDetailShown = true;
    }
    public void ShowMatchResult()
    {
        matchResutlContainer.transform.localScale = Vector3.zero;
        matchResutlContainer.transform.rotation = Quaternion.Euler(0, 0, -10f);

        matchResutlContainer.gameObject.SetActive(true);

        Sequence animSequence = DOTween.Sequence();

        animSequence.Append(matchResutlContainer.transform.DOScale(Vector3.one, 1.4f)
            .SetEase(Ease.OutBounce));

        animSequence.Join(matchResutlContainer.transform.DORotate(Vector3.zero, 1.1f)
            .SetEase(Ease.OutQuart));

    }
    public void HideCardDetail()
    {
        Sequence resultSequence = DOTween.Sequence();
        onHidingPanel = true;
        resultSequence.Append(cardDetailCanvasGroup.DOFade(0f, fadeDuration)).
            SetEase(Ease.InFlash).OnComplete(() =>
        {
            cardImage.sprite = null;
            t_cardName.text = string.Empty;
            t_cardValue.text = string.Empty;
            t_cardLevel.text = string.Empty;
            t_cardDescription.text = string.Empty;
            onHidingPanel = false;
            isCardDetailShown = false;
        });
    }
    public void SetPlayerUI(ClassSO playerClass, float HP, float ARMOR)
    {
        currentPlayerClass = playerClass;
        img_player.sprite = playerClass.ClassIcon;
        img_playerSkill.sprite = playerClass.SkillIcon;
        t_playerClass.text = playerClass.ClassName;
        t_playerHP.text = HP.ToString();
        t_playerArmor.text = ARMOR == 0 ? string.Empty : ARMOR.ToString();
        t_playerSkill.text = playerClass.Skill.SkillName;
        slider_playerHP.value = HP;
    }
    public void SetEnemyUI(ClassSO enemyClass, float HP, float ARMOR)
    {
        currentEnemyClass = enemyClass;
        img_enemy.sprite = enemyClass.ClassIcon;
        img_enemySkill.sprite = enemyClass.SkillIcon;
        t_enemyClass.text = enemyClass.ClassName;
        t_enemyHP.text = HP.ToString();
        t_enemyArmor.text = ARMOR == 0 ? string.Empty : ARMOR.ToString();
        t_enemySkill.text = enemyClass.Skill.SkillName;
        t_enemyMana.text = GameManager.Instance.enemy.currentMana.ToString() + "/" + GameManager.Instance.enemy.currentMaxMana.ToString();
        slider_enemyHP.value = HP;
    }

    public void InitManaOnBeginTurn(int maxMana)
    {
        Turn currentTurn = TurnManager.Instance.currentTurn;

        if (manaContainer.GetChildCount() > 0 && currentTurn == Turn.PLAYER)
        {
            foreach (Transform mana in manaContainer)
            {
                Debug.Log($"Destroying Mana Object : {mana.name}");
                Destroy(mana.gameObject);
            }
        }

        if (currentTurn == Turn.PLAYER)
        {
            for (int i = 0; i < maxMana; i++)
            {
                GameObject manaObj = Instantiate(manaPrefab, manaContainer);
            }
        }
        else if (currentTurn == Turn.ENEMY)
        {
            t_enemyMana.text = GameManager.Instance.enemy.currentMana.ToString() + "/" + maxMana.ToString();
        }
    }
    public void IncreaseMana(int addedAmount)
    {
        Turn currentTurn = TurnManager.Instance.currentTurn;

        if (currentTurn == Turn.ENEMY)
        {
            GameManager.Instance.enemy.currentMana += addedAmount;
            t_enemyMana.text = GameManager.Instance.enemy.currentMana.ToString() + "/" + GameManager.Instance.enemy.currentMaxMana.ToString();
        }
        else if (currentTurn == Turn.PLAYER)
        {
            for (int i = 0; i < addedAmount; i++)
            {
                GameObject manaObj = Instantiate(manaPrefab, manaContainer);
            }
        }
    }
    public void DecreaseMana(int usedAmount)
    {
        Turn currentTurn = TurnManager.Instance.currentTurn;

        switch (currentTurn)
        {
            case Turn.PLAYER:
                for (int i = 0; i < usedAmount; i++)
                {
                    if (manaContainer.childCount > 0)
                    {
                        GameObject manaToDestroy = manaContainer.GetChild(0).gameObject;
                        Debug.Log($"Mana Gameobject ({i}) : " + manaToDestroy.name);
                        DestroyImmediate(manaToDestroy);
                    }
                }
                break;
            case Turn.ENEMY:
                t_enemyMana.text = GameManager.Instance.enemy.currentMana.ToString() + "/" + GameManager.Instance.player.currentMaxMana.ToString();
                break;
        }
    }

    public void InitDeckOnBoard(DeckSO deckSO, Turn turn)
    {
        Deck deck = turn == Turn.PLAYER ? GameManager.Instance.playerDeck : GameManager.Instance.enemyDeck;
        Transform deckParent = turn == Turn.PLAYER ? playerDeckContainer : enemyDeckContainer;
        deck.SetDeckParent(deckParent);
        deck.InitDeck(deckSO);
    }

    public void EntityTakeDamage(Turn receiver, int currentHP, int currentShield)
    {
        if (receiver == Turn.PLAYER)
        {
            t_playerHP.text = currentHP.ToString();
            t_playerArmor.text = currentShield == 0 ? string.Empty : currentShield.ToString();
            slider_playerHP.value = currentHP;
        }
        else if (receiver == Turn.ENEMY)
        {
            t_enemyHP.text = currentHP.ToString();
            t_enemyArmor.text = currentShield == 0 ? string.Empty : currentShield.ToString();
            slider_enemyHP.value = currentHP;
        }
    }

    public void EntityGainShield(Turn receiver, int currentShield)
    {
        if (receiver == Turn.PLAYER)
        {
            t_playerArmor.text = currentShield == 0 ? string.Empty : currentShield.ToString();
        }
        else if (receiver == Turn.ENEMY)
        {
            t_enemyArmor.text = currentShield == 0 ? string.Empty : currentShield.ToString();
        }
    }
    public void UpdateTimerText(float val)
    {
        int minutes = Mathf.FloorToInt(val / 60f);
        int seconds = Mathf.FloorToInt(val % 60f);

        t_timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ToggleSetting()
    {
        settingContainer.gameObject.SetActive(true);
    }

    public void CloseSetting()
    {
        settingContainer.gameObject.SetActive(false);
    }

    public void ShowLeaderboard()
    {
        leaderboardContainer.transform.localPosition = hiddenPosition;

        Sequence resultSequence = DOTween.Sequence();

        resultSequence.Append(leaderboardContainer.transform.DOLocalMove(leaderboard_originalPosition, fadeDuration).SetEase(Ease.OutFlash));
        resultSequence.AppendInterval(shownDuration);
        //resultSequence.Append(leaderboardContainer.transform.DOLocalMove(hiddenPosition, fadeDuration * 0.7f).SetEase(Ease.InQuad));

        resultSequence.OnComplete(() => resultTextContainer.SetActive(false));
    }

    public void HideLeaderboard()
    {
        Sequence resultSequence = DOTween.Sequence();   

        resultSequence.Append(leaderboardContainer.transform.DOLocalMove(hiddenPosition, fadeDuration * 0.7f).SetEase(Ease.InQuad));
    }

    public void PlayAgain()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
        if (!isTransitioning && SceneTransitionManager.Instance != null)
        {
            isTransitioning = true;
            transform.DOScale(result_originalScale * 0.9f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    transform.DOScale(result_originalScale, 0.1f).SetEase(Ease.OutQuad);
                    SceneTransitionManager.Instance.TransitionToScene("Gameplay", TransitionType.Fade);
                });

        }
    }
    public void NavigateBackToMainMenu()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
        if (!isTransitioning && SceneTransitionManager.Instance != null)
        {
            isTransitioning = true;
            transform.DOScale(result_originalScale * 0.9f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    transform.DOScale(result_originalScale, 0.1f).SetEase(Ease.OutQuad);
                    SceneTransitionManager.Instance.TransitionToScene("MainMenu", TransitionType.Fade);
                });

        }
    }

    public void NavigateBackToPVEMap()
    {
        AudioManager.Instance.PlaySFX("Button-Click");
        if (!isTransitioning && SceneTransitionManager.Instance != null)
        {
            isTransitioning = true;
            transform.DOScale(result_originalScale * 0.9f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    transform.DOScale(result_originalScale, 0.1f).SetEase(Ease.OutQuad);
                    SceneTransitionManager.Instance.TransitionToScene("PVE_Map", TransitionType.Fade);
                });

        }
    }

    public void ShownTableCap()
    {
        tableCapContainer.DOLocalMove(tableCap_shownPosition, tableCap_moveDuration)
        .SetEase(Ease.OutQuad);
    }

    public void HideTableCap()
    {
        tableCapContainer.DOLocalMove(tableCap_hiddenPosition, tableCap_moveDuration)
            .SetEase(Ease.InQuad);
    }

}