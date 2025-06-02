using System.Collections.Generic;
using System;
using UnityEngine;

public class PlacementArea : MonoBehaviour
{
    public int maxCards = 3;

    public float cardSpacing = 0.5f;
    public float animationSpeed = 5.0f;
    public float xGap = 0.05f;
    public Transform placementParent;

    private List<Transform> cardOnBoards = new List<Transform>();
    private Queue<char> cardQueue = new Queue<char>();
    private bool isEnter = false;

    private int c = 0;
    private void Update()
    {
        UpdateCardPositions();
    }
    public void AddCard(Transform newCard)
    {
        if (newCard != null) 
        {
            Card card = newCard.GetComponent<Card>();
            if (c >= maxCards && card.cardData.CardType == CardType.Operator)
            {
                //Reach to maximum cards, can't add more number
                //need to add operator card to use more number cards
                c = 0;
            }
            else if(card.cardData.CardType == CardType.Number && c < maxCards)
            {
                cardQueue.Enqueue(card.cardData.cardValue);
                c++;
            }

            AddCardToBoard(newCard);
        }
    }

    private void AddCardToBoard(Transform newCard)
    {
        cardOnBoards.Add(newCard);
        newCard.SetParent(placementParent);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mouse"))
        {
            ActionManager.Instance.EnterPlacementArea();
            isEnter = true;
            Debug.Log("Enter Placement Area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isEnter)
        {
            ActionManager.Instance.ExitPlacementArea();
            isEnter = false;
            Debug.Log("Exit Placement Area");
        }
    }

    void UpdateCardPositions()
    {
        for (int i = 0; i < cardOnBoards.Count; i++)
        {
            Vector3 targetPos = CalculateTargetPosition(i);
            cardOnBoards[i].localPosition = Vector3.Lerp(cardOnBoards[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
        }
    }

    Vector3 CalculateTargetPosition(int index)
    {
        float totalWidth = (cardOnBoards.Count - 1) * cardSpacing;
        float startZ = -totalWidth / 2;
        float xOffset = index * xGap;

        return new Vector3(-xOffset, 0f, startZ + index * cardSpacing);
    }

    public bool IsReturnCardBackToHand(Transform newCard)
    {
        Card card = newCard.GetComponent<Card>();
        if (c >= maxCards && card.cardData.CardType == CardType.Number)
        {
            Debug.Log($"Cannot place number card: {c}/{maxCards} slots filled");
            return true;
        }

        return false;
    }
}
