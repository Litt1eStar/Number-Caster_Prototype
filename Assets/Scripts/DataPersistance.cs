using UnityEngine;

public class DataPersistance : MonoBehaviour
{
    public static DataPersistance Instance;

    public ClassSO enemyClass;
    public ClassSO playerClass;
    public Sprite gameplayBackgroundSprite;
    
    public Sprite playerProfileSprite;
    public string playerName;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
