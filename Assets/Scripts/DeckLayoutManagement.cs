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

    public Transform player1_deckPosition;

    public float dragHeight = 1.0f;
    public LayerMask cardLayerMask = -1;
    
    private List<Transform> handOfPlayer = new List<Transform>();

    public Camera mainCamera;
    private Transform draggedCard;
    private int draggedCardOriginalIndex;
    private Vector3 draggedCardOriginalPosition;
    private int insertIndex = -1;

    private void Start()
    {
        side = Turn.Player1;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard(Turn.Player1, deck);
 /*           GameObject m_newCard = Instantiate(newCard);
            AddCard(m_newCard);*/
        }
        HandleDragAndDrop();
        UpdateCardPositions();
    }
    public void DrawCard(Turn sender, Deck deckOfSender)
    {
        Transform destination = sender == Turn.Player1 ? player1_deckPosition : null; //null for now, can be set to Player2's deck position later
        GameObject drawedCard = deckOfSender.DrawCard();
        if (drawedCard != null)
        {
            AddCard(drawedCard);
        }
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
    void HandleDragAndDrop()
    {
        if(Input.GetMouseButtonDown(0) && draggedCard == null)
        {
            //StartDrag
            StartDrag();
        }

        if(Input.GetMouseButton(0) && draggedCard != null)
        {
            //ContinueDrag
            ContinueDrag();
        }

        if(Input.GetMouseButtonUp(0) && draggedCard != null)
        {
            //EndDrag
            EndDrag();
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
                Transform temp = draggedCard;
                RemoveCard(draggedCard.gameObject);
                placementArea.AddCard(temp);
                Debug.Log("Placed card on Placement Area");
                break;
            case false:
                if (insertIndex != draggedCardOriginalIndex)
                {
                    SwapCard(handOfPlayer, draggedCard, draggedCardOriginalIndex, insertIndex);

                    Debug.Log($"Card moved from index {draggedCardOriginalIndex} to {insertIndex}.");
                }
                break;
        }

        draggedCard = null;
        draggedCardOriginalIndex = -1;
        insertIndex = -1;
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
            Vector3 targetPos = CalculateTargetPosition(i);
            if(dragZ > targetPos.z)
            {
                insertIndex = i + 1;
            }
        }

        insertIndex = Mathf.Clamp(insertIndex, 0, handOfPlayer.Count - 1);
    }
    Vector3 CalculateTargetPosition(int index)
    {
        float totalWidth = (handOfPlayer.Count - 1) * cardSpacing;
        float startZ = -totalWidth / 2;
        float xOffset = index * xGap;

        return new Vector3(-xOffset, 0f, startZ + index * cardSpacing);
    }
    void UpdateCardPositions()
    {
        float totalWidth = (handOfPlayer.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < handOfPlayer.Count; i++)
        {
            if (handOfPlayer[i] == draggedCard) continue;

            Vector3 targetPos = CalculateTargetPosition(i);

            handOfPlayer[i].localPosition = Vector3.Lerp(handOfPlayer[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
            if (handOfPlayer[i] != draggedCard)
            {
                handOfPlayer[i].SetSiblingIndex(i);
            }
        }
    }




}
