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

    [Header("Reference Setting")]
    [SerializeField] private Transform placementParent;
    [SerializeField] private BoardUI boardUI;
    public Transform usedCardParent;

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
        if (IsLatestCardOperator())
        {
            //Give some feedback to player that they cannot use this button
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
        }
    }

    public void OnClickProtectButton()
    {
        if (IsLatestCardOperator())
        {
            //Give some feedback to player that they cannot use this button
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

    private void SendCardToUsedArea()
    {
        float yOffset = 0.01f;

        foreach (Transform card in cardOnBoards)
        {
            card.SetParent(usedCardParent);

            Vector3 targetPosition = new Vector3(0, GameManager.Instance.usedCardAreaYPosition, 0);
            card.DOLocalMove(targetPosition, GameManager.Instance.sendCardToUsedAreaAnimationSpeed * Time.deltaTime);
            GameManager.Instance.usedCardAreaYPosition += yOffset;
        }

        cardOnBoards.Clear();
        cardQueue.Clear();
        c = 0;
    }

    public bool IsLatestCardOperator()
    {
        if(cardOnBoards.Count <= 0) return false;

        Transform lastCard = cardOnBoards[cardOnBoards.Count - 1];
        Card card = lastCard.GetComponent<Card>();
        
        return card.cardData.CardType == CardType.Operator;
    }
}
