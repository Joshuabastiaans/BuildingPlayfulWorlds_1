 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    //Variables
    public Transform player;
    public float smooth = 0.3f;

    public float camDistanceX = 6.0f;
    public float camDistanceY = 7.0f;
    public float camDistanceZ = 6.0f;

    private Vector3 velocity = Vector3.zero;

    public float zoomSpeed = 2.0f;
    public float zoomMin = 40.0f;
    public float zoomMax = 80.0f;

    private bool isZoomOut = false;

    //Methods
    void Update ()
    {
        FollowPlayer();
        ZoomCam();
    }

    void FollowPlayer()
    {
        Vector3 pos = new Vector3();
        pos.x = player.position.x - camDistanceX;
        pos.y = player.position.y + camDistanceY;
        pos.z = player.position.z - camDistanceZ;
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smooth);
    }

    void ZoomCam()
    {
        // Check if the right mouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            isZoomOut = true;
        }
        // Check if the right mouse button is released
        if (Input.GetMouseButtonUp(1))
        {
            isZoomOut = false;
        }

        // Slowly zoom out the camera while the right mouse button is held down
        if (isZoomOut)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomMax, Time.deltaTime * zoomSpeed);
        }
        // Slowly reset the camera zoom when the right mouse button is released
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomMin, Time.deltaTime * zoomSpeed);
        }
    }
}
