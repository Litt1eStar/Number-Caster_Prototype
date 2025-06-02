using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private GameObject btnContainer;

    private void Start()
    {
        btnContainer.SetActive(false);
    }

    public void ShowButton()
    {
        btnContainer.SetActive(true);
    }

    public void HideButton()
    {
        btnContainer.SetActive(false);
    }   
}
