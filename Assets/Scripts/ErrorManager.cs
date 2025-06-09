using TMPro;
using UnityEngine;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager Instance { get { return _instance; } }
    private static ErrorManager _instance;

    [SerializeField] private TextMeshProUGUI t_error;
    private string errorMessage = string.Empty;

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

    public void SetErrorMessage(string errMsg)
    {
        errorMessage = errMsg;
        t_error.text = errorMessage;
    }
}