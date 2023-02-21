using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    private void OnTriggerEnter(Collider other)
    {
        enemyAI.OnHitWithAxe(other);
    }
}