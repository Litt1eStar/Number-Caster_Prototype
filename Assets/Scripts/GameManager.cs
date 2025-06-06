using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return instance; } }
    private static GameManager instance;

    [Header("Animation Reference")]
    public float sendCardToUsedAreaAnimationSpeed = 0.5f;
    public float usedCardAreaYPosition = 0.0f;

    [Header("Transform Reference")]
    public Transform placementParent;
    public Transform usedCardParent;

    [Header("Class Reference")]
    public BoardUI boardUI { get; private set;}
    public PlacementArea placementArea { get; private set; }

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
        if (boardUI == null)
        {
            boardUI = GameObject.FindFirstObjectByType<BoardUI>();
            if (boardUI == null)
            {
                Debug.LogError("BoardUI is not in scene.");
            }
        }

        if(placementArea == null)
        {
            placementArea = GameObject.FindFirstObjectByType<PlacementArea>();
            if (placementArea == null)
            {
                Debug.LogError("PlacementArea is not in scene.");
            }
        }
    }
}
