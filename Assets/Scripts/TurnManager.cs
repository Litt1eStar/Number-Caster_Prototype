using System.Collections;
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
        timer += Time.deltaTime;
        if(timer >= turnDuration)
        {
            EndTurn();
        }
    }
    public void EndTurn()
    {
        timer = 0f;
        StartCoroutine(EndTurnSequence());
    }
    public IEnumerator EndTurnSequence()
    {
        DrawCard(currentTurn);
        yield return new WaitForSeconds(1.5f); 
        SwitchTurn();
    }

    public void InitTurnSystem(Turn startingSide = Turn.PLAYER)
    {
        StartCoroutine(DrawInitialCards());
        timer = 0f;
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
        timer = 0f;
        StartTurn();
    }

    public void StartTurn()
    {
        //Increase Mana for 1 unit
        //Draw card
        Entity entity = currentTurn == Turn.PLAYER ? GameManager.Instance.player : GameManager.Instance.enemy;
        entity.IncreaseMaxMana();
        DrawCard(currentTurn);
    }

    public void DrawCard(Turn destination)
    {
        if (destination == Turn.PLAYER) GameManager.Instance.handController.DrawCardToPlayer();
        else if (destination == Turn.ENEMY) GameManager.Instance.handController.DrawCardToEnemy();
    }
}
