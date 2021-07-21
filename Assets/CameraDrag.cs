using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 lastPosition = new Vector3();

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return;
        }
        if (Input.GetMouseButton(2))
        {
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var move = lastPosition - position;
            transform.Translate(new Vector2(move.x, move.y));
            lastPosition = position;
        }
    }
}