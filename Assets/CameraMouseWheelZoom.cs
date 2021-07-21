using UnityEngine;

public class CameraMouseWheelZoom : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 10;
    private Camera zoomCamera;

    void Start()
    {
        zoomCamera = Camera.main;
    }

    void Update()
    {
        zoomCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        if (zoomCamera.orthographicSize < 1)
        {
            zoomCamera.orthographicSize = 1;
        }
    }
}
