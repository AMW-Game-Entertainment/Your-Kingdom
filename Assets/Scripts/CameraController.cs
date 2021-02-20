using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Zoom")]
    public float cameraSpeed;
    public float maxZoom;
    public float minZoom;

    private Camera cam;
    private Vector3 mouseOiriginPoint;
    private Vector3 cameraOffset;
    private bool isDragging;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        ZoomMap();
        DragMap();
    }

    /**
     * Zoom within the map
     * @return void
     */
    void ZoomMap()
    {
        // Get the current zoom calculation
        float currentZoom = cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * (cameraSpeed * cam.orthographicSize * .1f);
        // Assign it and be sure didnt surpass the min and max amount
        cam.orthographicSize = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    /**
     * Drag within the map
     * @return void
     */
    void DragMap()
    {
        // On let click
        if (Input.GetMouseButton(0) && !GameManager.instance.HasSelectedBuilding)
        {
            // Get the user mouse position and convert it to world position point
            Vector3 currentWorldPointAt = cam.ScreenToWorldPoint(Input.mousePosition);
            // Get the camera offset based current world point at, then subtract it to current camera position
            cameraOffset = (currentWorldPointAt - transform.position);

            if (!isDragging)
            {
                isDragging = true;
                mouseOiriginPoint = currentWorldPointAt;
            }

            if (isDragging)
                transform.position = mouseOiriginPoint - cameraOffset;
        }
        else if (isDragging)
        {
            isDragging = false;
        }
    }
}
