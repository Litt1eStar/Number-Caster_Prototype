using UnityEngine;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager Instance { get { return _instance; } }
    private static ErrorManager _instance;

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
}
