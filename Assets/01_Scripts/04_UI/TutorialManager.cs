using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject TutorialMoving;
    public GameObject TutorialZooming;
    public GameObject TutorialAttacking;

    private bool ZoomAlreadyActivated;
    private bool AttackAlreadyActivated;

    void Update()
    {
        if (TutorialMoving.activeSelf && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            Cursor.visible = false;
            TutorialMoving.SetActive(false);
        }

        if (TutorialZooming.activeSelf && Input.GetMouseButtonDown(1))
        {
            Cursor.visible = false;
            TutorialZooming.SetActive(false);
        }

        if (TutorialAttacking.activeSelf && Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
            TutorialAttacking.SetActive(false);
        }
    }

    public void TriggerEntered(string triggerName)
    {
        Debug.Log(triggerName);
        if (triggerName == "ZoomingTrigger"&& ZoomAlreadyActivated == false)
        {
            TutorialZooming.SetActive(true);
            ZoomAlreadyActivated = true;
        }
        else if (triggerName == "AttackTrigger"&& AttackAlreadyActivated == false)
        {
            TutorialAttacking.SetActive(true);
            AttackAlreadyActivated = true;
        }
    }
}