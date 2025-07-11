using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return instance; } }
    private static GameManager instance;

    [Header("Animation Reference")]
    public float sendCardToUsedAreaAnimationSpeed = 0.5f;
    public float usedCardAreaYPosition = 0.0f;

    [Header("Deck Reference")]
    public Deck playerDeck;
    public Deck enemyDeck;

    [Header("Transform Reference")]
    public Transform placementParent;
    public Transform playerUsedArea;
    public Transform enemyUsedArea;

    [Header("Class Reference")]
    public BoardUI boardUI;
    public PlacementArea placementArea;
    public HandController handController;

    [Header("Match Settings")]
    public Player player { get; private set; } = null;
    public Enemy enemy { get; private set; } = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Start()
    {
        Match match = new GameObject("Match").AddComponent<Match>();
        if (match != null)
        {
            match.Init();
        }
        else
        {
            Debug.LogError("Failed to create Match instance.");
        }

        if (boardUI == null)
        {
            boardUI = GameObject.FindFirstObjectByType<BoardUI>();
            if(boardUI == null)
            {
                Debug.LogError("BoardUI is not in scene.");
            }
        }

        if(placementArea == null)
        {
            placementArea = GameObject.FindFirstObjectByType<PlacementArea>();
            if(placementArea == null)
            {
                Debug.LogError("PlacementArea is not in scene.");
            }
        }
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void SetEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }
}
