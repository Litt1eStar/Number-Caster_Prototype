using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    [Header("Button Reference")]
    [SerializeField] private GameObject btnContainer;

    [Header("Result Text Reference")]
    [SerializeField] private GameObject resultTextContainer;
    [SerializeField] private CanvasGroup resultCanvasGroup;
    [SerializeField] private TextMeshProUGUI t_rawValue;
    [SerializeField] private TextMeshProUGUI t_cappedValue;
    [SerializeField] private TextMeshProUGUI t_timer;
    [SerializeField] private TextMeshProUGUI t_turnText;

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
    [SerializeField] private TextMeshProUGUI t_playerClass;
    [SerializeField] private TextMeshProUGUI t_playerHP;
    [SerializeField] private TextMeshProUGUI t_playerArmor;
    [SerializeField] private TextMeshProUGUI t_playerSkill;
    [SerializeField] private Transform playerDeckContainer;
    private ClassSO currentPlayerClass;

    [Header("Enemy Settings")]
    [SerializeField] private Image img_enemy;
    [SerializeField] private TextMeshProUGUI t_enemyClass;
    [SerializeField] private TextMeshProUGUI t_enemyHP;
    [SerializeField] private TextMeshProUGUI t_enemyArmor;
    [SerializeField] private TextMeshProUGUI t_enemySkill;
    [SerializeField] private Transform enemyDeckContainer;
    private ClassSO currentEnemyClass;

    public bool onHidingPanel { get; private set; } = false;
    public bool isCardDetailShown { get; private set; } = false;
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

        if(cardDetailCanvasGroup == null)
        {
            cardDetailCanvasGroup = cardDetailContainer.GetComponent<CanvasGroup>();
            if (cardDetailCanvasGroup == null)
            {
                cardDetailCanvasGroup = cardDetailContainer.AddComponent<CanvasGroup>();
            }
        }
        cardDetailCanvasGroup.alpha = 0f;
    }
    private void Update()
    {
        UpdateTurnText();
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

        resultSequence.Append(resultTextContainer.transform.DOLocalMove(originalPosition, fadeDuration).SetEase(Ease.OutFlash));
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
        t_playerClass.text = playerClass.ClassName;
        t_playerHP.text = HP.ToString();
        t_playerArmor.text = ARMOR == 0 ? string.Empty : ARMOR.ToString();
        t_playerSkill.text = playerClass.Skill.SkillName;
    }
    public void SetEnemyUI(ClassSO enemyClass, float HP, float ARMOR)
    {
        currentEnemyClass = enemyClass;
        img_enemy.sprite = enemyClass.ClassIcon;
        t_enemyClass.text = enemyClass.ClassName;
        t_enemyHP.text = HP.ToString();
        t_enemyArmor.text = ARMOR == 0 ? string.Empty : ARMOR.ToString();
        t_enemySkill.text = enemyClass.Skill.SkillName;
    }   
    public void InitDeckOnBoard(DeckSO deckSO, Turn turn)
    {
        Deck deck = turn == Turn.PLAYER ? GameManager.Instance.playerDeck : GameManager.Instance.enemyDeck;
        Transform deckParent = turn == Turn.PLAYER ? playerDeckContainer : enemyDeckContainer;
        deck.SetDeckParent(deckParent);
        deck.InitDeck(deckSO);

        /*GameObject obj = deckSO.cards[0];
        if (turn == Turn.PLAYER)
        {
            obj.transform.SetParent(playerDeckContainer);
        }
        else
        {
            obj.transform.SetParent(enemyDeckContainer);
        }

        obj.transform.localScale = Vector3.zero;*/
    }

    public void EntityTakeDamage(Turn receiver,int currentHP, int currentShield)
    {
        if(receiver == Turn.PLAYER)
        {
            t_playerHP.text = currentHP.ToString();
            t_playerArmor.text = currentShield == 0 ? string.Empty : currentShield.ToString();
        }
        else if (receiver == Turn.ENEMY)
        {
            t_enemyHP.text = currentHP.ToString();
            t_enemyArmor.text = currentShield == 0 ? string.Empty : currentShield.ToString();
        }
    }

    public void EntityGainShield (Turn receiver,int currentShield)
    {
        if(receiver == Turn.PLAYER)
        {
            t_playerArmor.text = currentShield == 0 ? string.Empty : currentShield.ToString();
        }else if(receiver == Turn.ENEMY)
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

    public void UpdateTurnText()
    {
        t_turnText.text = TurnManager.Instance.currentTurn == Turn.PLAYER ? "Player" : "Enemy";
    }
}   
