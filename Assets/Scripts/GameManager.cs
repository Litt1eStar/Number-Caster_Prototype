using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return instance; } }
    private static GameManager instance;

    [Header("Animation Reference")]
    public float sendCardToUsedAreaAnimationSpeed = 10.0f;
    public float usedCardAreaYPosition = 0.0f;

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


}
