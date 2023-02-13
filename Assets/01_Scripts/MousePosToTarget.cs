using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosToTarget : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        foreach (Transform parent in transform)
        {
            parent.position = mousePos;
        }
    }
}
