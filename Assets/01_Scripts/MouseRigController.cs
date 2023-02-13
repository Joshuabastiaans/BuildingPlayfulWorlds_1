using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MouseRigController : MonoBehaviour
{
    Rig rig;
    private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;

    public Vector3 mousePos { get; private set; } 

    void Start()
    {
        // Get the Rig component from the player object
        rig = GetComponent<Rig>();
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        RigWeight();
        MousePosition();

    }


    private void MousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            mousePos = raycastHit.point;
        }
    }

    void RigWeight()
    {
        Vector3 playerPos = player.position;

        // Calculate the angle between the mouse and the player
        float angle = Vector3.Angle(mousePos - playerPos, transform.forward);

        // If the angle is greater than 90 degrees, the cursor is behind the player
        if (angle > 90)
        {
            // Gradually decrease the weight of the rig to 0
            rig.weight = Mathf.Lerp(rig.weight, 0, Time.deltaTime);
        }
        else
        {
            // Gradually increase the weight of the rig to 1
            rig.weight = Mathf.Lerp(rig.weight, 1, Time.deltaTime);
        }
    }
}
