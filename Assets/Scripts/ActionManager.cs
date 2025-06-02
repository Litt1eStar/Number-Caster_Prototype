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
    public void EnterPlacementArea()
    {
        isInPlacementArea = true;
    }

    public void ExitPlacementArea()
    {
        isInPlacementArea = false;
    }
}
