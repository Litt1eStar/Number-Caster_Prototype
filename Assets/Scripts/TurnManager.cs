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


    [SerializeField] private Turn currentTurn;
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
            SwitchTurn();
            timer = 0f;
        }
    }
    public void InitTurnSystem(Turn startingSide = Turn.PLAYER)
    {
        currentTurn = startingSide;
        timer = 0f;
    }
    public void SwitchTurn()
    {
        currentTurn = currentTurn == Turn.PLAYER ? Turn.ENEMY : Turn.PLAYER;
        timer = 0f;
        Debug.Log("Switched turn to: " + currentTurn);
    }
}
