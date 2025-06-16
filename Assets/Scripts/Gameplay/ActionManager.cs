using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance { get { return _instance; } }
    private static ActionManager _instance;

    public bool isInPlacementArea { get; private set; } = false;

    private void Awake()
    {
        if(_instance == null)
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartMatch();
        }
    }
    public void EnterPlacementArea()
    {
        isInPlacementArea = true;
    }

    public void ExitPlacementArea()
    {
        isInPlacementArea = false;
    }

    public void StartMatch()
    {
        Match match = Instantiate(new GameObject("Match")).AddComponent<Match>();
        if (match != null)
        {
            match.Init();
        }
        else
        {
            Debug.LogError("Failed to create Match instance.");
        }
    }
}
