using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn
{
    PLAYER,
    ENEMY
}
public class TurnManager : MonoBehaviour
{
    private static TurnManager _instance;
    public static TurnManager Instance { get { return _instance; } }

    public Turn currentTurn;
    [SerializeField] private float timer = 0f;
    [SerializeField] private float turnDuration = 60f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        GameManager.Instance.boardUI.UpdateTimerText(timer);

        if (timer <= 0)
        {
            EndTurn();
        }
    }
    public void EndTurn()
    {
        StartCoroutine(SendRemainingCardOnBoardBackToHand());   
        timer = turnDuration;
        StartCoroutine(EndTurnSequence());
    }
    public IEnumerator SendRemainingCardOnBoardBackToHand()
    {
        List<Transform> cardsOnBoard = GameManager.Instance.placementArea.GetCardsOnBoard();
        
        if(cardsOnBoard.Count <= 0) yield return null;  
        
        List<Transform> cardsCopy = new List<Transform>(cardsOnBoard);
        Debug.Log($"Card on Board : {cardsCopy.Count}");
        foreach (Transform card in cardsCopy)
        {
            GameManager.Instance.placementArea.SendCardBackToHand(card);
            Debug.Log($"Send {card.gameObject.name}");
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(3f);
    }
    public IEnumerator EndTurnSequence()
    {
        DrawCard(currentTurn);
        yield return new WaitForSeconds(1.5f); 
        SwitchTurn();
    }

    public void InitTurnSystem(Turn startingSide = Turn.PLAYER)
    {
        timer = turnDuration;
        StartCoroutine(DrawInitialCards());
        GameManager.Instance.boardUI.UpdateTimerText(timer);
        currentTurn = startingSide;
    }
    IEnumerator DrawInitialCards()
    {
        yield return StartCoroutine(DrawCardsForPlayer(Turn.ENEMY, 5));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DrawCardsForPlayer(Turn.PLAYER, 5));
        yield return new WaitForSeconds(1f);
        StartTurn();
    }
    IEnumerator DrawCardsForPlayer(Turn player, int cardCount)
    {
        for (int i = 0; i < cardCount; i++)
        {
            currentTurn = player;
            DrawCard(player);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SwitchTurn()
    {
        currentTurn = currentTurn == Turn.PLAYER ? Turn.ENEMY : Turn.PLAYER;
        StartTurn();
    }

    public void StartTurn()
    {
        //Increase Mana for 1 unit
        //Draw card
        timer = turnDuration;
        Entity entity = currentTurn == Turn.PLAYER ? GameManager.Instance.player : GameManager.Instance.enemy;
        entity.IncreaseMaxMana();
        DrawCard(currentTurn);

        if (currentTurn == Turn.ENEMY)
        {
            GameManager.Instance.enemy.StartBotTurn();
        }
    }

    public void DrawCard(Turn destination)
    {
        if (destination == Turn.PLAYER) GameManager.Instance.handController.DrawCardToPlayer();
        else if (destination == Turn.ENEMY) GameManager.Instance.handController.DrawCardToEnemy();
    }
}
