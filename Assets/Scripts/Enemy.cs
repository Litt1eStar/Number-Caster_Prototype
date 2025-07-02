using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private List<Transform> handOfEnemy;
    private List<Transform> handOfPlayer;
    private List<Transform> usedCard;
    private float turnStartTime;
    private int currentCardNumberCount = 0;
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
        turnStartTime = Time.time;
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
        int cardPlayed = 0;

        while (usedCard.Count > 0)
        {
            Card randomCard = usedCard[0].GetComponent<Card>();
            Debug.LogWarning($"Playing card: {randomCard.cardData.cardName}, Cost: {randomCard.cardData.cost}");
            
            if(GameManager.Instance.enemy.currentMana - randomCard.cardData.cost < 0)
            {
                Debug.Log("Not enough mana to play card: " + randomCard.cardData.cardName);
                usedCard.RemoveAt(0); 
                continue; // Skip this card if not enough mana
            }

            float turnTimeRemaining = 90f - (Time.time - turnStartTime);
            float cardDelay = BotCardTimingLogic.CalculateCardPlayDelay(usedCard.Count, turnTimeRemaining, randomCard);

            Debug.Log($"Delay : {cardDelay}");

            bool isEnoughMana = GameManager.Instance.enemy.currentMana >= randomCard.cardData.cost;
            if (!isEnoughMana)
            {
                Debug.LogError($"Not enough Mana to play");
                yield return null;
            }

            if (GameManager.Instance.placementArea.IsBoardEmpty())
            {
                //Case for first card played, it can be only number and skill card
                switch (randomCard.cardData.CardType)
                {
                    case CardType.Number:
                        UseNumberCard(randomCard);
                        Debug.Log("Play Number Card: " + randomCard.cardData.cardName);
                        break;
                    case CardType.Operator:
                        //We're going to find first card in used card that is number card
                        //If it's not found, then we can't play operator card
                        break;
                    case CardType.Skill:
                        Debug.Log("Use Skill Card");
                        GameManager.Instance.handController.UseSkillCard(randomCard, randomCard.cardData.cost);
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
                            UseNumberCard(randomCard);
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
                                UseOperatorCard(randomCard); 
                            }
                            else
                            {
                                Debug.Log("Should not play operator card, next card is not a number card.");
                            }
                        }
                        break;
                    case CardType.Skill:
                        Debug.Log("Use Skill Card");
                        GameManager.Instance.handController.UseSkillCard(randomCard, randomCard.cardData.cost);
                        break;
                }
            }

            usedCard.RemoveAt(0);
            cardPlayed++;
            yield return new WaitForSeconds(cardDelay); //how long should it take to play each card
        }

        Debug.Log("All cards played.");
        yield return new WaitForSeconds(5f); // Wait for a moment before ending the turn
        ActionDecision();
        yield return null;
    }
    
    private void ActionDecision()
    {
        bool isAttacking = false;
        if (isAttacking)
        {
            GameManager.Instance.placementArea.OnClickAttackButton();
        }
        else
        {
            GameManager.Instance.placementArea.OnClickProtectButton();
        }
    }
    private void UseNumberCard(Card card)
    {
        GameManager.Instance.handController.SendCardToPlacementArea(card.cardData.cost, card.transform);
        card.FlipCardToAnotherSide();
        currentCardNumberCount++;
    }

    private void UseOperatorCard(Card card)
    {
        GameManager.Instance.handController.SendCardToPlacementArea(card.cardData.cost, card.transform);
        card.FlipCardToAnotherSide();
        currentCardNumberCount = 0;
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
        float strategicDelay = GetStrategicDelay(currentCard);
        float finalDelay = (baseDelay * cardTypeMultiplier) + strategicDelay;

        finalDelay = Mathf.Clamp(finalDelay, MIN_CARD_DELAY, MAX_CARD_DELAY);
        finalDelay += Random.Range(-0.2f, 0.3f);
        
        return Mathf.Max(finalDelay, MIN_CARD_DELAY);
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

    private static float GetStrategicDelay(Card currentCard)
    {
        float strategicDelay = 0f;

        if (currentCard.cardData.CardType == CardType.Skill)
        {
            strategicDelay += 0.5f;
        }

        if (GameManager.Instance.placementArea.IsBoardEmpty())
        {
            strategicDelay += 0.3f;
        }
        
        float hpPercentage = (float)GameManager.Instance.enemy.HP / 20;
        if (hpPercentage < 0.3f)
        {
            strategicDelay *= 0.5f;
        }
        else if (hpPercentage > 0.8f)
        {
            strategicDelay *= 1.2f; 
        }

        return strategicDelay;
    }
}