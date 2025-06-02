using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance { get { return _instance; } }
    private static ActionManager _instance;

    private bool isInPlacementArea = false;

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
    private void EnterPlacementArea()
    {
        isInPlacementArea = true;
    }

    public void ExitPlacementArea()
    {
        isInPlacementArea = false;
    }
}
