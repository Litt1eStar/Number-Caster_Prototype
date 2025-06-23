using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private List<Transform> handOfEnemy;
    private List<Transform> handOfPlayer;
    public override void SetUI()
    {
        base.SetUI();
        GameManager.Instance.boardUI.SetEnemyUI(classSO, HP, ARMOR);
        GameManager.Instance.boardUI.InitDeckOnBoard(deckSO, Turn.ENEMY);
    }

    public void StartBotTurn()
    {
        handOfEnemy = GameManager.Instance.handController.HandOfEnemy;
        StartCoroutine(ExecuteBotTurn());
    }

    IEnumerator ExecuteBotTurn()
    {
        int randomAmount = Random.Range(1, handOfEnemy.Count);
        Debug.Log($"Amount of Card to Player in this turn : {randomAmount}/{handOfEnemy.Count}");
        List<Transform> usedCard = PickRandomCardOnHand(handOfEnemy, randomAmount);

        int numbersToPlay = 3; //Count number card in usedCard
        int operatorsToPlay = 3; //Count operator card in usedCard
        int maxPlayableNumbers = (operatorsToPlay + 1) * GameManager.Instance.placementArea.MaxCardOnCardSequence(); //Max number of card that can be played in this turn

        if (numbersToPlay > maxPlayableNumbers)
        {
            int excessNumber = numbersToPlay - maxPlayableNumbers;
            //Remove excess numbers from usedCard
        }
        yield return null;
    }

    public List<Transform> PickRandomCardOnHand(List<Transform> hands, int amountToPick)
    {
        return null;
    }
}
