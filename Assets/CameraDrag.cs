using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 originalCameraPosition = new Vector3();
    private Vector3 startDragPosition = new Vector3();
    private Vector3? lastMousePosition = null;

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(2))
        {
            originalCameraPosition = transform.position;
            startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lastMousePosition = null;
            return;
        }
        if (Input.GetMouseButton(2))
        {
            var mousePosition = Input.mousePosition;
            if (lastMousePosition != null && Vector3.Distance(mousePosition, (Vector3)lastMousePosition) < 1)
            {
                return;
            }
            lastMousePosition = mousePosition;
            var dragDiff = startDragPosition - Camera.main.ScreenToWorldPoint(mousePosition);
            var newPosition = originalCameraPosition + dragDiff;
            transform.position = new Vector3(newPosition.x, newPosition.y, originalCameraPosition.z);
        }
    }
}