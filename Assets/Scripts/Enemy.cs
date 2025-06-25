using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private List<Transform> handOfEnemy;
    private List<Transform> handOfPlayer;
    private List<Transform> usedCard;
    public override void SetUI()
    {
        base.SetUI();
        GameManager.Instance.boardUI.SetEnemyUI(classSO, HP, ARMOR);
        GameManager.Instance.boardUI.InitDeckOnBoard(deckSO, Turn.ENEMY);
    }

    public void StartBotTurn()
    {
        handOfEnemy = GameManager.Instance.handController.HandOfEnemy;
        usedCard = new List<Transform>(handOfEnemy);
        StartCoroutine(ExecuteBotTurn());
    }

    IEnumerator ExecuteBotTurn()
    {
        int randomAmount = Random.Range(1, handOfEnemy.Count);
        usedCard = PickRandomCardOnHand(handOfEnemy, randomAmount);
        Debug.Log($"Amount of Card to Player in this turn : {usedCard.Count}/{handOfEnemy.Count}");

        int numbersToPlay = CountCardByType(usedCard, CardType.Number); //Count number card in usedCard
        int operatorsToPlay = CountCardByType(usedCard, CardType.Operator); //Count operator card in usedCard
        int maxPlayableNumbers = (operatorsToPlay + 1) * GameManager.Instance.placementArea.MaxCardOnCardSequence(); //Max number of card that can be played in this turn

        Debug.Log($"Amount of Numbers Card: {numbersToPlay}, Amount of Operators Card: {operatorsToPlay}, Max playable Cards: {maxPlayableNumbers}");

        if (numbersToPlay > maxPlayableNumbers)
        {
            int excessNumber = numbersToPlay - maxPlayableNumbers;
            //Remove excess numbers from usedCard
        }

        yield return StartCoroutine(PlayCard());
        yield return new WaitForSeconds(10f); //delay after playing cards
        TurnManager.Instance.EndTurn();
    }

    IEnumerator PlayCard()
    {
        int currentCardNumberCount = 0; 

        while (usedCard.Count > 0)
        {
            Card randomCard = usedCard[0].GetComponent<Card>();
            if (GameManager.Instance.placementArea.IsBoardEmpty())
            {
                //Case for first card played, it can be only number and skill card
                switch (randomCard.cardData.CardType)
                {
                    case CardType.Number:
                        GameManager.Instance.handController.SendCardToPlacementArea(usedCard[0].transform);
                        currentCardNumberCount++;
                        Debug.Log("Play Number Card: " + randomCard.cardData.cardName);
                        break;
                    case CardType.Operator:
                        //We're going to find first card in used card that is number card
                        //If it's not found, then we can't play operator card
                        break;
                    case CardType.Skill:
                        Debug.Log("Use Skill Card");
                        GameManager.Instance.handController.UseSkillCard(randomCard);
                        break;
                }
            }
            else
            {
                //Case for subsequent cards played, it can be number, operator, or skill card
                Debug.Log("Play subsequent card: " + randomCard.cardData.cardName);
                switch (randomCard.cardData.CardType)
                {
                    case CardType.Number:
                        bool isReachLimit = currentCardNumberCount >= GameManager.Instance.placementArea.MaxCardOnCardSequence();
                        if (!isReachLimit)
                        {
                            GameManager.Instance.handController.SendCardToPlacementArea(usedCard[0].transform);
                            currentCardNumberCount++;
                        }
                        else
                        {
                            Debug.Log("Cannot play more number cards, limit reached.");
                            //Find first operator card in usedCard
                            //If found then play it, else skip
                        }
                        break;
                    case CardType.Operator:
                        if (usedCard.Count > 1)
                        {
                            bool shouldPlayOperatorCard = usedCard[1].GetComponent<Card>().cardData.CardType == CardType.Number;
                            if (shouldPlayOperatorCard)
                            {
                                GameManager.Instance.handController.SendCardToPlacementArea(usedCard[0].transform);
                                currentCardNumberCount = 0;
                            }
                            else
                            {
                                Debug.Log("Should not play operator card, next card is not a number card.");
                            }
                        }
                        break;
                    case CardType.Skill:
                        Debug.Log("Use Skill Card");
                        GameManager.Instance.handController.UseSkillCard(randomCard);
                        break;
                }
            }

            usedCard.RemoveAt(0);
            yield return new WaitForSeconds(Random.Range(0.5f, 3f)); //how long should it take to play each card
        }

        Debug.Log("All cards played.");
        yield return new WaitForSeconds(5f); // Wait for a moment before ending the turn
        bool isAttacking = false;
        if(isAttacking)
        {
            GameManager.Instance.placementArea.OnClickAttackButton();
        }
        else
        {
            GameManager.Instance.placementArea.OnClickProtectButton();
        }
        yield return null;
    }

    private Card FindFirstCardByType(List<Transform> cards, CardType type)
    {
        foreach (Transform card in cards)
        {
            if (card.GetComponent<Card>().cardData.CardType == type)
            {
                return card.GetComponent<Card>();
            }
        }
        return null;
    }
    public List<Transform> PickRandomCardOnHand(List<Transform> hands, int amountToPick)
    {

        return hands.GetRange(0, amountToPick);
    }

    private int CountCardByType(List<Transform> cards, CardType type)
    {
        int count = 0;
        foreach (Transform card in cards)
        {
            if (card.GetComponent<Card>().cardData.CardType == type)
            {
                count++;
            }
        }
        return count;
    }
}

public class BotCardTimingLogic
{
    private const float TURN_DURATION = 90f;
    private const float MIN_CARD_DELAY = 0.5f;
    private const float MAX_CARD_DELAY = 4f;
    private const float BUFFER_TIME = 10f; //time for attack, protect decision making

    public static float CalculateCardPlayDelay(int cardsRemaining, float turnTimeRemaining, Card currentCard)
    {
        float baseDelay = (turnTimeRemaining - BUFFER_TIME) / cardsRemaining;
        float cardTypeMultiplier = GetCardTypeMultiplier(currentCard.cardData.CardType);

        return 0;
    }

    private static float GetCardTypeMultiplier(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Number:
                return 0.8f; // Base multiplier for number cards
            case CardType.Operator:
                return 1.2f; // Slightly longer delay for operator cards
            case CardType.Skill:
                return 1.5f; // Longer delay for skill cards due to their complexity
            default:
                return 1.0f; // Default multiplier
        }
    }
}