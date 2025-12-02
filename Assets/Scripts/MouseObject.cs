using UnityEngine;

public class MouseObject : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        mousePos.z = Vector3.Distance(transform.position, mainCamera.transform.position);

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        worldPos.y = transform.position.y;

        transform.position = worldPos;
    }
}
