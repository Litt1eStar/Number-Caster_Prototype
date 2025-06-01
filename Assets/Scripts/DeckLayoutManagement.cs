using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class DeckLayoutManagement : MonoBehaviour
{
    public float cardSpacing = 0.5f;    
    public float animationSpeed = 5.0f;
    public float xGap = 0.05f;

    public PlayerSide side; //for test

    public Transform player1_deckPosition;

    public float dragHeight = 1.0f;
    public LayerMask cardLayerMask = -1;

    public GameObject newCard;
    
    private List<Transform> cards_player01 = new List<Transform>();
    private List<Transform> working_cards = new List<Transform>();

    public Camera mainCamera;
    private Transform draggedCard;
    private int draggedCardOriginalIndex;
    private Vector3 draggedCardOriginalPosition;
    private int insertIndex = -1;

    private void Start()
    {
        side = PlayerSide.Player1;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject m_newCard = Instantiate(newCard);
            AddCard(m_newCard);
        }
        HandleDragAndDrop();
        UpdateCardPositions();
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
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cardLayerMask))
        {
            Transform hitCard = hit.collider.transform;

            if (working_cards.Contains(hitCard))
            {
                draggedCard = hitCard;
                draggedCardOriginalIndex = working_cards.IndexOf(draggedCard);
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
        Vector3 mousePos = Input.mousePosition; 
        mousePos.z = mainCamera.WorldToScreenPoint(draggedCard.position).z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        
        Vector3 localPos = draggedCard.parent.InverseTransformPoint(worldPos);
        localPos.x = 0.03f;

        draggedCard.localPosition = localPos;
        draggedCard.SetAsFirstSibling();
        CalculateInsertIndex();
    }
    void EndDrag()
    {
        if(insertIndex != draggedCardOriginalIndex)
        {
            working_cards.RemoveAt(draggedCardOriginalIndex);
            working_cards.Insert(insertIndex, draggedCard);

            Debug.Log($"Card moved from index {draggedCardOriginalIndex} to {insertIndex}.");
        }

        draggedCard = null;
        draggedCardOriginalIndex = -1;
        insertIndex = -1;
    }

    void CalculateInsertIndex()
    {
        if(working_cards.Count <= 1)
        {
            insertIndex = 0; 
            return;
        }

        float dragZ = draggedCard.localPosition.z;
        insertIndex = 0;

        for (int i = 0; i < working_cards.Count; i++)
        {
            if (working_cards[i] == draggedCard) continue;
            Vector3 targetPos = CalculateTargetPosition(i);
            if(dragZ > targetPos.z)
            {
                insertIndex = i + 1;
            }
        }

        insertIndex = Mathf.Clamp(insertIndex, 0, working_cards.Count - 1);
    }
    Vector3 CalculateTargetPosition(int index)
    {
        float totalWidth = (working_cards.Count - 1) * cardSpacing;
        float startZ = -totalWidth / 2;
        float xOffset = index * xGap;

        return new Vector3(-xOffset, 0f, startZ + index * cardSpacing);
    }
    void UpdateCardPositions()
    {
        float totalWidth = (working_cards.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < working_cards.Count; i++)
        {
            if (working_cards[i] == draggedCard) continue;

            Vector3 targetPos = CalculateTargetPosition(i);

            working_cards[i].localPosition = Vector3.Lerp(working_cards[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
            if (working_cards[i] != draggedCard)
            {
                working_cards[i].SetSiblingIndex(i);
            }
        }
    }

    public void AddCard(GameObject card)
    {   
        card.GetComponent<Card>().SetOwner(side);
        card.transform.SetParent(player1_deckPosition);
        card.transform.localRotation = Quaternion.identity;

        working_cards.Add(card.transform);
        UpdateCardPositions();
    }

    public void RemoveCard(GameObject card)
    {
        if (working_cards.Contains(card.transform))
        {
            working_cards.Remove(card.transform);
            Destroy(card);
        }
    }

}
