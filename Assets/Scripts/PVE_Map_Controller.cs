using UnityEngine;
using UnityEngine.SceneManagement;

public class PVE_Map_Controller : MonoBehaviour
{
    public void StartMatch()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
