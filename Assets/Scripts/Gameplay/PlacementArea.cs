using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class PlacementArea : MonoBehaviour
{
    [Header("Animation Setting")]
    [SerializeField] private int maxCards = 3;
    [SerializeField] private float cardSpacing = 0.5f;
    [SerializeField] private float animationSpeed = 5.0f;
    [SerializeField] private float xGap = 0.05f;
    [SerializeField] private float dragHeight = 1.0f;

    [Header("Gameplay Setting")]
    [SerializeField] private Camera mainCamera;

    [Header("Reference Setting")]
    [SerializeField] private LayerMask cardLayerMask = -1;
    [SerializeField] private HandController deckLayoutManagement;

    private BoardUI boardUI;
    private List<Transform> cardOnBoards = new List<Transform>();
    private Queue<char> cardQueue = new Queue<char>();
    private bool isEnter = false;
    private int currentCardNumberCount = 0;

    private PlacementArea placementArea;
    private Transform draggedCard = null;
    private Vector3 draggedCardOriginalPosition;
    private int draggedCardOriginalIndex;
    private int amountOfRemainingCards = 0;

    private void Start()
    {
        InitializeComponent();
        ResetBoard();
    }
    private void Update()
    {
        UpdateCardPositions();
        HandleDragAndDrop();
        UpdateButtonVisibility();
    }
    #region Initialization Component
    private void InitializeComponent()
    {
        boardUI = GameManager.Instance.boardUI;
        placementArea = GameManager.Instance.placementArea;

        ValidateComponent();
    }
    private void ValidateComponent()
    {
        if (boardUI == null)
        {
            ErrorManager.Instance.SetErrorMessage("BoardUI is not assigned in GameManager.");
            return;
        }
        if (placementArea == null)
        {
            ErrorManager.Instance.SetErrorMessage("PlacementArea is not assigned in GameManager.");
            return;
        }
    }
    private void ResetBoard()
    {
        cardOnBoards.Clear();
        cardQueue.Clear();
        currentCardNumberCount = 0;
    }
    #endregion
    #region Drag and Drop System
    private void HandleDragAndDrop()
    {
        if (Input.GetMouseButtonDown(0) && draggedCard == null) StartDrag();
        if (Input.GetMouseButton(0) && draggedCard != null) ContinueDrag();
        if (Input.GetMouseButtonUp(0) && draggedCard != null) EndDrag();
    }
    private void StartDrag()
    {
        Transform cardToDrag = GetCardUnderMouse();
        if (cardToDrag == null || !cardOnBoards.Contains(cardToDrag)) return;

        InitializeDragState(cardToDrag);
        LiftCard(cardToDrag);
    }
    private void ContinueDrag()
    {
        //If we are dragging a card, we need to update its position
        //We will use the mouse position to calculate the new position of the card
        //and set x position to make card appear on top of other cards
        //Then calculate the insert index based on the position of dragged card

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 localPos = draggedCard.parent.InverseTransformPoint(mouseWorldPosition);
        draggedCard.localPosition = localPos;
    }
    private void EndDrag()
    {
        switch (ActionManager.Instance.isInPlacementArea)
        {
            case true:
                AnimateCardBackToBoard();
                break;
            case false:
                SendCardBackToHand();
                break;
        }

        //Clear drag state after placing the card
        ClearDraggedCardState();
    }
    private void InitializeDragState(Transform card)
    {
        draggedCard = card;
        draggedCardOriginalPosition = card.localPosition;
    }
    private void LiftCard(Transform card)
    {
        Vector3 liftedPosition = draggedCardOriginalPosition;
        liftedPosition.y += dragHeight;
        card.localPosition = liftedPosition;
        card.SetAsLastSibling();
    }
    private Transform GetCardUnderMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cardLayerMask))
        {
            return hit.collider.transform;
        }

        return null;
    }
    private void AnimateCardBackToBoard()
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
                // Update hand positions to ensure proper layout
                UpdateCardPositions();
            });

        // Optional: Add a slight bounce effect or scale animation
        cardToAnimate.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);
    }
    private void ClearDraggedCardState()
    {
        draggedCard = null;
        draggedCardOriginalIndex = -1;
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.WorldToScreenPoint(draggedCard.position).z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
    private void UpdateButtonVisibility()
    {
        if (!IsBoardEmpty()) boardUI.ShowButton();
        else boardUI.HideButton();
    }
    #endregion
    #region Card Management
    public void AddCard(Transform newCard)
    {
        if (newCard != null) 
        {
            Card card = newCard.GetComponent<Card>();

            bool isReachLimit = currentCardNumberCount >= maxCards;

            if (DeckHelper.IsOperatorCard(card)) currentCardNumberCount = 0;
            if (isReachLimit && DeckHelper.IsOperatorCard(card)) currentCardNumberCount = 0;
            else if(DeckHelper.IsNumberCard(card) && !isReachLimit) currentCardNumberCount++;
           
            AddCardToBoard(newCard, card);
        }
    }
    private void AddCardToBoard(Transform newCard, Card card)
    {
        cardQueue.Enqueue(card.cardData.cardValue);

        cardOnBoards.Add(newCard);
        newCard.SetParent(GameManager.Instance.placementParent);
    }
    private void UpdateCardPositions()
    {
        for (int i = 0; i < cardOnBoards.Count; i++)
        {
            Vector3 targetPos = DeckHelper.CalculateTargetPosition(i, cardOnBoards, cardSpacing, xGap);
            cardOnBoards[i].localPosition = Vector3.Lerp(cardOnBoards[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
        }
    }
    private void SendCardToUsedArea()
    {
        float yOffset = 0.01f;

        foreach (Transform card in cardOnBoards)
        {
            card.SetParent(GameManager.Instance.usedCardParent);

            Vector3 targetPosition = new Vector3(0, GameManager.Instance.usedCardAreaYPosition, 0);
            card.DOLocalMove(targetPosition, GameManager.Instance.sendCardToUsedAreaAnimationSpeed);
            GameManager.Instance.usedCardAreaYPosition += yOffset;
        }

        ResetBoard();
    }
    private void SendCardBackToHand()
    {
        Card removedCard = draggedCard.GetComponent<Card>();
        if (CanRemoveCard(draggedCard))
        {
            currentCardNumberCount = amountOfRemainingCards;
            cardOnBoards.Remove(draggedCard);
            cardQueue.Dequeue();
            deckLayoutManagement.AddCard(draggedCard.gameObject, TurnManager.Instance.currentTurn);
            ClearDraggedCardState();
        }
        else
        {
            ErrorManager.Instance.SetErrorMessage("Can't Remove this card.");
        }
    }
    private bool CanRemoveCard(Transform card)
    {
        if (!cardOnBoards.Contains(card))
            return false;

        Card cardComponent = card.GetComponent<Card>();

        // If it's a number card, we can always remove it
        if (cardComponent.cardData.CardType == CardType.Number)
        {
            amountOfRemainingCards = currentCardNumberCount - 1;
            return true;
        }

        // If it's an operator card, we need to check if removal maintains the maxCards limit
        if (cardComponent.cardData.CardType == CardType.Operator)
        {
            int cardIndex = cardOnBoards.IndexOf(card);

            int numberCardsBefore = 0;
            for (int i = 0; i < cardIndex; i++)
            {
                Card beforeCard = cardOnBoards[i].GetComponent<Card>();
                if (beforeCard.cardData.CardType == CardType.Number)
                    numberCardsBefore++;
            }

            int numberCardsAfter = 0;
            for (int i = cardIndex + 1; i < cardOnBoards.Count; i++)
            {
                Card afterCard = cardOnBoards[i].GetComponent<Card>();
                if (afterCard.cardData.CardType == CardType.Number)
                    numberCardsAfter++;
            }

            amountOfRemainingCards = numberCardsBefore + numberCardsAfter;
            return (numberCardsBefore + numberCardsAfter) <= maxCards;
        }

        return false;
    }
    #endregion
    #region Board Action
    public bool IsReturnCardBackToHand(Transform newCard)
    {
        Card card = newCard.GetComponent<Card>();
        if (currentCardNumberCount >= maxCards && card.cardData.CardType == CardType.Number)
        {
            ErrorManager.Instance.SetErrorMessage($"Cannot place number card: {currentCardNumberCount}/{maxCards} slots filled");
            return true;
        }

        return false;
    }
    public bool IsPreviousCardOperator()
    {
        if (cardOnBoards.Count == 0) return false;

        Card lastCard = cardOnBoards[cardOnBoards.Count - 1].GetComponent<Card>();
        return lastCard.cardData.CardType == CardType.Operator;
    }   
    public bool IsBoardEmpty() => cardOnBoards.Count == 0;
    public void OnClickAttackButton()
    {
        if (IsLatestCardOperator())
        {
            //Give some feedback to player that they cannot use this button
            ErrorManager.Instance.SetErrorMessage("Can't Calculate - Lastest card is Operator");
            return;
        }

        if(BoardCalculation.CalculateBoardValue(cardQueue, out int result))
        {
            //Send card on board to used card area
            SendCardToUsedArea();
            //Cap Value of result
            int cappedValue = ValueCapper.CapValue(result);
            //Use result to Attack Enemy
            boardUI.ShowResult(result, cappedValue);
            Entity target = TurnManager.Instance.currentTurn == Turn.PLAYER ? GameManager.Instance.enemy : GameManager.Instance.player;
            target.TakeDamage(cappedValue);
        }
    }
    public void OnClickProtectButton()
    {
        if (IsLatestCardOperator())
        {
            //Give some feedback to player that they cannot use this button
            ErrorManager.Instance.SetErrorMessage("Can't Calculate - Lastest card is Operator");
            return;
        }

        if (BoardCalculation.CalculateBoardValue(cardQueue, out int result))
        {
            //Send card on board to used card area
            SendCardToUsedArea();
            //Cap Value of result
            int cappedValue = ValueCapper.CapValue(result);
            //Use result to Create Shield for Player
            boardUI.ShowResult(result, cappedValue);
        }
    }
    public bool IsLatestCardOperator()
    {
        if(cardOnBoards.Count <= 0) return false;

        Transform lastCard = cardOnBoards[cardOnBoards.Count - 1];
        Card card = lastCard.GetComponent<Card>();
        
        return card.cardData.CardType == CardType.Operator;
    }
    #endregion
    #region Unity Trigger Events
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mouse"))
        {
            ActionManager.Instance.EnterPlacementArea();
            isEnter = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (isEnter)
        {
            ActionManager.Instance.ExitPlacementArea();
            isEnter = false;
        }
    }

    #endregion
}
