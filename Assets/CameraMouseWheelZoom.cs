using System.Collections;
using System.Collections.Generic;
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
    }
}
