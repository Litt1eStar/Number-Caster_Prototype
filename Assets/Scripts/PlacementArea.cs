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
    public BoardUI boardUI;    

    private List<Transform> cardOnBoards = new List<Transform>();
    private Queue<char> cardQueue = new Queue<char>();
    private bool isEnter = false;

    private int c = 0;
    private void Update()
    {
        UpdateCardPositions();
        if (!IsBoardEmpty())
        {
            boardUI.ShowButton();
        }
        else
        {
            boardUI.HideButton();
        }
    }
    public void AddCard(Transform newCard)
    {
        if (newCard != null) 
        {
            Card card = newCard.GetComponent<Card>();

            bool isReachLimit = c >= maxCards;

            if (DeckHelper.IsOperatorCard(card)) c = 0;
            if (isReachLimit && DeckHelper.IsOperatorCard(card)) c = 0;
            else if(DeckHelper.IsNumberCard(card) && !isReachLimit) c++;
           
            AddCardToBoard(newCard, card);
        }
    }

    private void AddCardToBoard(Transform newCard, Card card)
    {
        cardQueue.Enqueue(card.cardData.cardValue);

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
            Vector3 targetPos = DeckHelper.CalculateTargetPosition(i, cardOnBoards, cardSpacing, xGap);
            cardOnBoards[i].localPosition = Vector3.Lerp(cardOnBoards[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
        }
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

    public bool IsPreviousCardOperator()
    {
        if (cardOnBoards.Count == 0) return false;

        Card lastCard = cardOnBoards[cardOnBoards.Count - 1].GetComponent<Card>();
        return lastCard.cardData.CardType == CardType.Operator;
    }   

    public bool IsBoardEmpty() => cardOnBoards.Count == 0;

    public void OnClickAttackButton()
    {
        int processedDamage = BoardCalculation.CalculateBoardValue(cardQueue);
    }

    public void OnClickProtectButton()
    {
        int processedProtection = BoardCalculation.CalculateBoardValue(cardQueue);
    }
}
