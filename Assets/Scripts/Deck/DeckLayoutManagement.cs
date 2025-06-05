using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
public class DeckLayoutManagement : MonoBehaviour
{
    [SerializeField] private Deck deck;

    public float cardSpacing = 0.5f;    
    public float animationSpeed = 5.0f;
    public float rotateSpeed = 10f;
    public float xGap = 0.05f;

    public Turn side; //for test
    public PlacementArea placementArea;
    public BoardUI boardUI;

    public Transform player1_deckPosition;
    public Transform usedCardParent;

    public float dragHeight = 1.0f;
    public LayerMask cardLayerMask = -1;
    
    private List<Transform> handOfPlayer = new List<Transform>();

    public Camera mainCamera;
    private Transform draggedCard;
    private int draggedCardOriginalIndex;
    private Vector3 draggedCardOriginalPosition;
    private int insertIndex = -1;

    private Card shownCard = null;
    private bool isCardDetailShown = false; 

    private void Start()
    {
        side = Turn.Player1;
    }
    void Update()
    {
        //FOR TESTING PURPOSE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard(Turn.Player1, deck);
        }

        HandleRightClick();
        HandleDragAndDrop();
        UpdateCardPositions();
    }
    private void HandleDragAndDrop()
    {
        if (Input.GetMouseButtonDown(0) && draggedCard == null) StartDrag();
        if (Input.GetMouseButton(0) && draggedCard != null) ContinueDrag();
        if (Input.GetMouseButtonUp(0) && draggedCard != null) EndDrag();   
    }

    private void HandleRightClick()
    {
        if ((Input.GetMouseButtonDown(1) && draggedCard == null) && !isCardDetailShown)
        {
            //Show card Detail
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cardLayerMask))
            {
                shownCard = hit.collider.transform.GetComponent<Card>();
                Debug.Log("Right click on card");
                boardUI.ShowCardDetail(shownCard);
                isCardDetailShown = true;
            }
        }else if((Input.GetMouseButtonDown(1) && draggedCard == null) && isCardDetailShown)
        {
            shownCard = null;
            boardUI.HideCardDetail();
            isCardDetailShown = false;
        }
    }
    public void DrawCard(Turn sender, Deck deckOfSender)
    {
        GameObject drawedCard = deckOfSender.DrawCard();
        if (drawedCard != null) AddCard(drawedCard);
    }
    public void AddCard(GameObject card)
    {
        card.GetComponent<Card>().SetOwner(side);
        card.transform.SetParent(player1_deckPosition);

        card.transform.DOLocalRotate(new Vector3(0, 0, 180), rotateSpeed).SetEase(Ease.OutQuart);

        handOfPlayer.Add(card.transform);
        UpdateCardPositions();
    }


    public void RemoveCard(GameObject card)
    {
        if (handOfPlayer.Contains(card.transform))
        {
            handOfPlayer.Remove(card.transform);
            card.transform.SetParent(null);
            UpdateCardPositions();
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
                if (placementArea.IsReturnCardBackToHand(draggedCard))
                {
                    AnimateCardBackToHand();
                    Debug.LogWarning("Card returned to hand - placement area full");
                    return;
                }

                Card card = draggedCard.GetComponent<Card>();
                if (DeckHelper.IsOperatorCard(card) && (placementArea.IsPreviousCardOperator() || placementArea.IsBoardEmpty()))
                {
                    AnimateCardBackToHand();
                    Debug.LogWarning("Cannot place another Operator card here.");
                    return;
                }

                if (card.cardData.CardType == CardType.Skill)
                {
                    UseSkillCard(card);
                    SendCardToUsedArea(draggedCard);
                }
                else if (DeckHelper.ValidCardTypeOnBoard(card))
                {
                    SendCardToPlacementArea();
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

    private void UseSkillCard(Card card)
    {
        Debug.Log("Use skill card");
    }
    private void SendCardToPlacementArea()
    {
        Transform temp = draggedCard;
        RemoveCard(draggedCard.gameObject);
        placementArea.AddCard(temp);
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
                UpdateCardPositions();
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
    void UpdateCardPositions()
    {
        for (int i = 0; i < handOfPlayer.Count; i++)
        {
            if (handOfPlayer[i] == draggedCard) continue;

            Vector3 targetPos = DeckHelper.CalculateTargetPosition(i, handOfPlayer, cardSpacing, xGap);

            handOfPlayer[i].localPosition = Vector3.Lerp(handOfPlayer[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
            if (handOfPlayer[i] != draggedCard)
            {
                handOfPlayer[i].SetSiblingIndex(i);
            }
        }
    }

    public void SendCardToUsedArea(Transform card)
    {
        float yOffset = 0.01f;

        RemoveCard(card.gameObject);
        card.SetParent(placementArea.usedCardParent);

        Vector3 targetPosition = new Vector3(0, placementArea.usedCardAreaYPosition, 0);
        card.DOLocalMove(targetPosition, placementArea.sendCardToUsedAreaAnimationSpeed * Time.deltaTime);
        placementArea.usedCardAreaYPosition += yOffset;

        ClearDraggedCardState();
    }




}
