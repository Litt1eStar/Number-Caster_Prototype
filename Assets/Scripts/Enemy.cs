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
        while (usedCard.Count > 0)
        {
            if (usedCard[0].GetComponent<Card>().cardData.CardType != CardType.Skill)
            {
                GameManager.Instance.handController.SendCardToPlacementArea(usedCard[0]);
            }
            else
            {
                Debug.Log("Use Skill Card");
            }

            usedCard.RemoveAt(0);  //Remove card after playing it
            yield return new WaitForSeconds(Random.Range(0.5f, 3f));
        }

        Debug.Log("All cards played.");
        yield return null;
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
