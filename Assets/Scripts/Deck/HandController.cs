using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
public class HandController : MonoBehaviour
{
    [SerializeField] private Deck player_deck;
    [SerializeField] private Deck enemy_deck;

    [Header("Animation Setting")]
    [SerializeField] private float cardSpacing = 0.5f;
    [SerializeField] private float animationSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float xGap = 0.05f;
    [SerializeField] private float dragHeight = 1.0f;

    [Header("Gameplay Setting")]
    [SerializeField] private Turn side; //for test
    [SerializeField] private Camera mainCamera;

    [Header("Reference Setting")]
    [SerializeField] private Transform player_handParent;
    [SerializeField] private Transform enemy_handParent;
    [SerializeField] private LayerMask cardLayerMask = -1;
    
    private List<Transform> handOfPlayer = new List<Transform>();
    private List<Transform> handOfEnemy = new List<Transform>();
    private Vector3 draggedCardOriginalPosition;
    private Transform draggedCard;
    private int draggedCardOriginalIndex;
    private int insertIndex = -1;

    private PlacementArea placementArea;
    private BoardUI boardUI;
    private Card shownCard = null;
    private bool isCardDetailShown = false;

    private void Start()
    {
        side = Turn.PLAYER;
        boardUI = GameManager.Instance.boardUI;
        placementArea = GameManager.Instance.placementArea;
    }
    void Update()
    {
        HandleRightClick();
        if (boardUI.isCardDetailShown) return;

        HandleDragAndDrop();

        UpdateCardPositions(TurnManager.Instance.currentTurn);
    }
    private void HandleDragAndDrop()
    {
        if (Input.GetMouseButtonDown(0) && draggedCard == null) StartDrag();
        if (Input.GetMouseButton(0) && draggedCard != null) ContinueDrag();
        if (Input.GetMouseButtonUp(0) && draggedCard != null) EndDrag();   
    }
    private void HandleRightClick()
    {
        if ((Input.GetMouseButtonDown(1) && draggedCard == null) && !isCardDetailShown && !boardUI.onHidingPanel)
        {
            //Show card Detail
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cardLayerMask))
            {
                shownCard = hit.collider.transform.GetComponent<Card>();
                boardUI.ShowCardDetail(shownCard);
                isCardDetailShown = true;
            }
        }else if((Input.GetMouseButtonDown(1) && draggedCard == null) && isCardDetailShown && !boardUI.onHidingPanel)
        {
            shownCard = null;
            boardUI.HideCardDetail();
            isCardDetailShown = false;
        }
    }
    public void DrawCard(Turn destination, Deck deckOfSender)
    {
        GameObject drawedCard = deckOfSender.DrawCard();
        if (drawedCard != null)
        {
            AudioManager.Instance.PlaySFX("Draw-Card");
            AddCard(drawedCard, destination);
        }
    }
    public void DrawCardToPlayer()
    {
        DrawCard(Turn.PLAYER, player_deck);
    }
    public void DrawCardToEnemy()
    {
        DrawCard(Turn.ENEMY, enemy_deck);
    }
    public void AddCard(GameObject card, Turn destination)
    {
        Transform destinationHand = destination == Turn.PLAYER ? player_handParent : enemy_handParent;
        List<Transform> targetHand = destination == Turn.PLAYER ? handOfPlayer : handOfEnemy;
        Vector3 cardRotation = destination == Turn.PLAYER ? new Vector3(0, 0, 180) : new Vector3(0, 0, 0);

        card.GetComponent<Card>().SetOwner(destination, rotateSpeed);
        card.transform.SetParent(destinationHand);
        //card.transform.DOLocalRotate(cardRotation, rotateSpeed).SetEase(Ease.OutQuart);

        targetHand.Add(card.transform);
        UpdateCardPositions(destination);
    }
    public void RemoveCard(GameObject card)
    {
        List<Transform> targetHand = TurnManager.Instance.currentTurn == Turn.PLAYER ? handOfPlayer : handOfEnemy;
        if (targetHand.Contains(card.transform))
        {
            targetHand.Remove(card.transform);
            card.transform.SetParent(null);
            UpdateCardPositions(TurnManager.Instance.currentTurn);
        }
    }
    void StartDrag()
    {
        //If player click on a card, start dragging it
        //To drag a card, we need to find the card under the mouse cursor 
        //using raycast to achieve this action
        //If the card is found, we keep a reference of position and index of the card
        //Then lift it up

        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cardLayerMask))
        {
            Transform hitCard = hit.collider.transform;

            if (handOfPlayer.Contains(hitCard))
            {
                //Set state when start drag
                draggedCard = hitCard;
                draggedCardOriginalIndex = handOfPlayer.IndexOf(draggedCard);
                draggedCardOriginalPosition = draggedCard.localPosition;

                Vector3 liftedPos = draggedCardOriginalPosition;
                liftedPos.y += dragHeight;
                draggedCard.localPosition = liftedPos;

                draggedCard.SetAsLastSibling();
            }
        }
    }
    void ContinueDrag()
    {
        //If we are dragging a card, we need to update its position
        //We will use the mouse position to calculate the new position of the card
        //and set x position to make card appear on top of other cards
        //Then calculate the insert index based on the position of dragged card

        Vector3 mousePos = Input.mousePosition; 
        mousePos.z = mainCamera.WorldToScreenPoint(draggedCard.position).z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        
        Vector3 localPos = draggedCard.parent.InverseTransformPoint(worldPos);
        localPos.x = 0.03f;

        draggedCard.localPosition = localPos;

        CalculateInsertIndex();
    }
    void EndDrag()
    {
        switch (ActionManager.Instance.isInPlacementArea)
        {
            case true:
                Entity entity = TurnManager.Instance.currentTurn == Turn.PLAYER ? GameManager.Instance.player : GameManager.Instance.enemy;
                int cardCost = draggedCard.GetComponent<Card>().cardData.cost;
                
                if (entity.currentMana - cardCost < 0)
                {
                    Debug.LogError("Not enough mana to play this card.");
                    AnimateCardBackToHand();
                    AudioManager.Instance.PlaySFX("Invalid-Card");
                    return;
                }

                if (placementArea.IsReturnCardBackToHand(draggedCard))
                {
                    AnimateCardBackToHand();
                    ErrorManager.Instance.SetErrorMessage("Card returned to hand - placement area full");
                    AudioManager.Instance.PlaySFX("Invalid-Card");
                    return;
                }

                Card card = draggedCard.GetComponent<Card>();
                if (DeckHelper.IsOperatorCard(card) && (placementArea.IsPreviousCardOperator() || placementArea.IsBoardEmpty()))
                {
                    AnimateCardBackToHand();
                    ErrorManager.Instance.SetErrorMessage("Cannot place Operator card here.");
                    AudioManager.Instance.PlaySFX("Invalid-Card");
                    return;
                }

                if (card.cardData.CardType == CardType.Skill)
                {
                    UseSkillCard(card, cardCost);
                }
                else if (DeckHelper.ValidCardTypeOnBoard(card))
                {
                    SendCardToPlacementArea(cardCost, draggedCard);
                }
                break;
            case false:
                if (insertIndex != draggedCardOriginalIndex)
                {
                    SwapCard(handOfPlayer, draggedCard, draggedCardOriginalIndex, insertIndex);
                }
                else
                {
                    AnimateCardBackToHand();
                    return;
                }
                break;
        }

        //Clear drag state after placing the card
        ClearDraggedCardState();
    }
    private void ClearDraggedCardState()
    {
        draggedCard = null;
        draggedCardOriginalIndex = -1;
        insertIndex = -1;
    }
    public void UseSkillCard(Card card, int cardCost)
    {
        switch (card.cardData.SkillName)
        {
            case "Mystical Discovery":
                StartCoroutine(ShownSkillCard(card));
                StartCoroutine(MysticalDiscoverySkill());
                break;
            case "Healing Ritual":
                StartCoroutine(ShownSkillCard(card));
                Entity target = TurnManager.Instance.currentTurn == Turn.PLAYER ? GameManager.Instance.player : GameManager.Instance.enemy;
                target.Heal(2);
                Debug.Log("Using Healing Ritual skill");
                break;
        }

        SendCardToUsedArea(card.transform);
        ReduceMana(TurnManager.Instance.currentTurn, cardCost);
    }
    private IEnumerator ShownSkillCard(Card card)
    {
        boardUI.ShowCardDetail(card);
        yield return new WaitForSeconds(1f);
        boardUI.HideCardDetail();
    }
    private IEnumerator MysticalDiscoverySkill()
    {
        for (int i = 0; i < 3; i++)
        {
            DrawCardToPlayer();
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void SendCardToPlacementArea(int cardCost, Transform objToSend = null)
    {
        if(objToSend == null)
        {
            Transform temp = draggedCard;
            RemoveCard(draggedCard.gameObject);
            placementArea.AddCard(temp);
     
            ReduceMana(TurnManager.Instance.currentTurn, cardCost);
        }
        else
        {
            Transform temp = objToSend;
            RemoveCard(objToSend.gameObject);
            placementArea.AddCard(temp);
            ReduceMana(TurnManager.Instance.currentTurn, cardCost);
        }

        AudioManager.Instance.PlaySFX("Placing-Card");

    }

    public void ReduceMana(Turn turn, int cost)
    {
        Entity target = turn == Turn.PLAYER ? GameManager.Instance.player : GameManager.Instance.enemy;
        if (target == null) return;

        target.UseCard(cost);
    }
    void AnimateCardBackToHand()
    {
        if (draggedCard == null) return;

        Transform cardToAnimate = draggedCard;
        Vector3 originalPos = draggedCardOriginalPosition;

        // Animate the card back to its original position
        cardToAnimate.DOLocalMove(originalPos, 0.3f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // Reset drag state after animation completes
                draggedCard = null;
                draggedCardOriginalIndex = -1;
                insertIndex = -1;

                // Update hand positions to ensure proper layout
                UpdateCardPositions(Turn.PLAYER);
            });

        // Optional: Add a slight bounce effect or scale animation
        cardToAnimate.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);
    }
    void SwapCard(List<Transform> container, Transform objToInsert, int from, int to)
    {
        container.RemoveAt(from);
        container.Insert(to, objToInsert);
    }
    void CalculateInsertIndex()
    {
        //If amount of card on hand <= 1
        //Then we can insert at index 0
        //Otherwise, loop to working card to compare z position of dragged card with other cards
        //And update insert index accordingly
        //Finally, clamp insert index to be within the range of 0 to working_cards.Count - 1
        if(handOfPlayer.Count <= 1)
        {
            insertIndex = 0; 
            return;
        }

        float dragZ = draggedCard.localPosition.z;
        insertIndex = 0;

        for (int i = 0; i < handOfPlayer.Count; i++)
        {
            if (handOfPlayer[i] == draggedCard) continue;
            Vector3 targetPos = DeckHelper.CalculateTargetPosition(i, handOfPlayer, cardSpacing, xGap);
            if(dragZ > targetPos.z)
            {
                insertIndex = i + 1;
            }
        }

        insertIndex = Mathf.Clamp(insertIndex, 0, handOfPlayer.Count - 1);
    }
    void UpdateCardPositions(Turn turn)
    {
        List<Transform> targetHand = turn == Turn.PLAYER ? handOfPlayer : handOfEnemy;

        for (int i = 0; i < targetHand.Count; i++)
        {
            if (targetHand[i] == draggedCard) continue;

            Vector3 targetPos = DeckHelper.CalculateTargetPosition(i, targetHand, cardSpacing, xGap);

            targetHand[i].localPosition = Vector3.Lerp(targetHand[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
            if (targetHand[i] != draggedCard)
            {
                targetHand[i].SetSiblingIndex(i);
            }
        }
    }

    public void SendCardToUsedArea(Transform card)
    {
        float yOffset = 0.01f;

        RemoveCard(card.gameObject);
        card.SetParent(GameManager.Instance.playerUsedArea);

        Vector3 targetPosition = new Vector3(0, GameManager.Instance.usedCardAreaYPosition, 0);
        card.DOLocalMove(targetPosition, GameManager.Instance.sendCardToUsedAreaAnimationSpeed);
        GameManager.Instance.usedCardAreaYPosition += yOffset;

        ClearDraggedCardState();
    }

    public List<Transform> HandOfEnemy => handOfEnemy;
    public List<Transform> HandOfPlayer => handOfPlayer;
}
