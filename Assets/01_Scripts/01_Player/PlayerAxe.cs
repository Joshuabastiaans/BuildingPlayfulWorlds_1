using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAxe : MonoBehaviour
{
    [SerializeField] private AnimationAndMovementController playerAnimation;

    private void OnTriggerEnter(Collider other)
    {
        playerAnimation.OnHitWithAxe(other);
    }
}
