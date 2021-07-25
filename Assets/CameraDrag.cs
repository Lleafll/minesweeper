using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    // Source: https://www.youtube.com/watch?v=0G4vcH9N0gc

    Vector3 touchStart;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
        }
    }
}