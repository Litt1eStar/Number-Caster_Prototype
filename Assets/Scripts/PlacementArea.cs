using UnityEngine;

public class PlacementArea : MonoBehaviour
{
    private bool isEnter = false;   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mouse"))
        {
            ActionManager.Instance.EnterPlacementArea();
            isEnter = true;
            Debug.Log("Enter Placement Area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isEnter)
        {
            ActionManager.Instance.ExitPlacementArea();
            isEnter = false;
            Debug.Log("Exit Placement Area");
        }
    }
}
