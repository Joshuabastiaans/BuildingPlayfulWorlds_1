using UnityEngine;

public class TriggerSignal : MonoBehaviour
{
    public TutorialManager tutorialManager;
    string triggerName;

    private void OnTriggerEnter(Collider other)
    {
        string triggerName = this.gameObject.name;
        if (other.tag == "Player")
        {
            tutorialManager.TriggerEntered(triggerName);
        }
    }
}