using UnityEngine;

public class DataPersistance : MonoBehaviour
{
    public static DataPersistance Instance;

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
