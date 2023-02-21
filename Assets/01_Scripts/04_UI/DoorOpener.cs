using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public GameObject ClosedDoorObject;
    public GameObject OpenDoorObject;

    public void OpenDoor()
    {
        StartCoroutine(WaitForOpenDoor());
    }

    public IEnumerator WaitForOpenDoor()
    {
        yield return new WaitForSecondsRealtime(3);
        EnemyAI[] enemyAIs = GameObject.FindObjectsOfType<EnemyAI>();
        if (enemyAIs.Length == 0)
        {
            OpenDoorObject.SetActive(true);
            ClosedDoorObject.SetActive(false);
        }
    }
}