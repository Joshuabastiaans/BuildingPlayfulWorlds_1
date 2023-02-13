using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameObject TutorialMoving;
    public GameObject TutorialZooming;
    public GameObject TutorialAttacking;

    public Collider TutorialAttackingCollider;
    public Collider TutorialZoomingCollider;


    private void Start()
    {
        TutorialAttackingCollider = GetComponent<Collider>();
        TutorialZoomingCollider = GetComponent<Collider>();
    }
    void Update()
    {
        if (TutorialMoving.activeSelf && Input.GetMouseButton(0))
        {
            Cursor.visible = false;
            TutorialMoving.SetActive(false);
        }        
        
        if (TutorialZooming.activeSelf && Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            TutorialZooming.SetActive(false);
        }

        if (TutorialAttacking.activeSelf && Input.GetMouseButton(0))
        {
            Cursor.visible = false;
            TutorialAttacking.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision dataFromCollision)
    {
        Debug.Log(dataFromCollision.gameObject.name);
        Debug.Log("wow");
        if (dataFromCollision.gameObject.name == "ZoomingBox")
        {
            TutorialZooming.SetActive(true);
            TutorialZoomingCollider.enabled = !TutorialZoomingCollider.enabled;
            Debug.Log("lol");
        }

        if (dataFromCollision.gameObject.name == "AttackBox")
        {
            TutorialAttacking.SetActive(true);
            TutorialAttackingCollider.enabled = !TutorialAttackingCollider.enabled;
        }
    }
}
