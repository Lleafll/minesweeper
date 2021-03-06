using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 1;
    [SerializeField] MineFieldManager mineFieldmanager;
    private float buttonDownDuration = 0;
    [SerializeField] private float directionThreshold = 0.01F;
    private bool isButtonDown = false;
    [SerializeField] private float longPressDurationInSeconds = 0.4F;
    [SerializeField] private int maxCameraSize = 50;
    private bool dragging = false;

    void Update()
    {
        if (Input.touchSupported)
        {
            UpdatedTouch();
        }
        else
        {
            UpdatedMouse();
        }
    }

    private void UpdatedTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragging = true;
            buttonDownDuration = 0;
            if (!ClickedOnUI())
            {
                isButtonDown = true;
            }
        }
        if (Input.touchCount == 2)
        {
            dragging = false;
            isButtonDown = false;
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);
            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            var prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            var ratio = prevMagnitude / currentMagnitude;
            zoom(ratio);
        }
        else if (Input.GetMouseButton(0))
        {
            if (dragging)
            {
                Drag();
            }
            if (isButtonDown)
            {
                buttonDownDuration += Time.deltaTime;
                if (buttonDownDuration >= longPressDurationInSeconds)
                {
                    ExecuteClick(false);
                }
            }
        }
        else if (isButtonDown && Input.GetMouseButtonUp(0))
        {
            ExecuteClick(true);
            dragging = false;
        }
    }

    private void UpdatedMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isButtonDown = true;
        }
        if (Input.GetMouseButton(0))
        {
            Drag();
        }
        if (Input.GetMouseButton(1))
        {
            ExecuteClick(false);
        }
        else if (isButtonDown && Input.GetMouseButtonUp(0))
        {
            ExecuteClick(true);
        }
        zoomMouseWheel(Input.GetAxis("Mouse ScrollWheel"));
    }

    private void Drag()
    {
        var direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Camera.main.transform.position += direction;
        if (direction.magnitude >= directionThreshold)
        {
            isButtonDown = false;
        }
        var position = Camera.main.transform.position;
        var (rows, columns) = mineFieldmanager.Length();
        position.x = Mathf.Clamp(position.x, 0, columns);
        position.y = Mathf.Clamp(position.y, -rows, 0);
        Camera.main.transform.position = position;
    }

    private void ExecuteClick(bool directClick)
    {
        if (ClickedOnUI())
        {
            return;
        }
        isButtonDown = false;
        mineFieldmanager.ExecuteClick(directClick);
    }

    private static bool ClickedOnUI()
    {
        // Check mouse
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        // Check touch
        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
            {
                return true;
            }
        }
        return false;
    }

    private void zoom(float scale)
    {
        Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize * scale, zoomOutMin);
        Camera.main.orthographicSize = System.Math.Min(maxCameraSize, Camera.main.orthographicSize);
    }

    private void zoomMouseWheel(float increment)
    {
        Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - increment, zoomOutMin);
        Camera.main.orthographicSize = System.Math.Min(maxCameraSize, Camera.main.orthographicSize);
    }
}