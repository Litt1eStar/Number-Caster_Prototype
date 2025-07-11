using UnityEngine;

public class DataPersistance : MonoBehaviour
{
    public static DataPersistance Instance;

    public ClassSO enemyClass;
    public ClassSO playerClass;
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
