using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionExclusion : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;

    void Start()
    {
        // Get the collider components for the two objects
        Collider collider1 = object1.GetComponent<Collider>();
        Collider collider2 = object2.GetComponent<Collider>();

        // Exclude the two colliders from interacting with each other
        Physics.IgnoreCollision(collider1, collider2);
    }
}
