using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 1;
    [SerializeField] MineFieldManager mineFieldmanager;
    private float buttonDownDuration = 0;
    private float directionThreshold = 0.01F;
    private bool isButtonDown = false;
    [SerializeField] private float longPressDurationInSeconds = 0.4F;

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
            buttonDownDuration = 0;
            isButtonDown = true;
        }
        if (Input.touchCount == 2)
        {
            isButtonDown = false;
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);
            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            var prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            var difference = currentMagnitude - prevMagnitude;
            zoom(difference * 0.01f);
        }
        else if (Input.GetMouseButton(0))
        {
            drag();
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
            drag();
        }
        if (Input.GetMouseButton(1))
        {
            ExecuteClick(false);
        }
        else if (isButtonDown && Input.GetMouseButtonUp(0))
        {
            ExecuteClick(true);
        }
        zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    private void drag()
    {
        var direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Camera.main.transform.position += direction;
        if (direction.magnitude >= directionThreshold)
        {
            isButtonDown = false;
        }
    }

    private void ExecuteClick(bool directClick)
    {
        isButtonDown = false;
        mineFieldmanager.ExecuteClick(directClick);
    }

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - increment, zoomOutMin);
    }
}