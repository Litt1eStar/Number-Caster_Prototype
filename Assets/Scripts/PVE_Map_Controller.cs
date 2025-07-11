using UnityEngine;
using UnityEngine.SceneManagement;

public class PVE_Map_Controller : MonoBehaviour
{
    public ClassSO enemyClass;
    public ClassSO playerClass;
    public void StartMatch()
    {
        DataPersistance.Instance.enemyClass = enemyClass;
        DataPersistance.Instance.playerClass = playerClass;

        SceneManager.LoadScene("Gameplay");
    }
}
