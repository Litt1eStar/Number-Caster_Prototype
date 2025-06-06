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
    public BoardUI boardUI;
    public PlacementArea placementArea;

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
