using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    PlayerHealth playerHealth;

    // The enemy's field of view
    public float fieldOfView = 110f;
    // The distance at which the enemy can detect the player
    public float detectionRange = 10f;
    public float breakfreeRange = 12f;
    // The distance at which the enemy can attack the player
    public float attackRange = 3f;
    // The speed at which the enemy moves
    public float moveSpeed = 3f;
    // The speed at which the enemy rotates
    public float rotationSpeed = 3f;

    // The amount of damage taken when hit by an enemy attack
    public int damageTaken = 10;

    private Transform player;
    private Animator animator;
    public NavMeshAgent navMeshAgent;
    private int attackTriggerHash;
    private int chaseTriggerHash;
    private int patrolTriggerHash;
    private int hitTriggerHash;
    private int dieTriggerHash;

    public float walkPointRange;
    bool walkPointSet;
    Vector3 walkPoint;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public GameObject weapon;
    public Collider weaponCollider;

    public LayerMask whatIsGround;

    bool hasSeenPlayer;

    // The maximum health of the enemy
    public int maxHealth = 100;
    // The current health of the enemy
    private int currentHealth;

    // Declare a variable to store the direction to the player
    Vector3 direction;

    public string tutorialSceneName = "Tutorial";
    public bool tutorialIsCompleted;

    void Awake()
    {
        attackTriggerHash = Animator.StringToHash("Attack");
        chaseTriggerHash = Animator.StringToHash("Chase");
        patrolTriggerHash = Animator.StringToHash("Patrol");
        hitTriggerHash = Animator.StringToHash("Hit");
        dieTriggerHash = Animator.StringToHash("Die");
        animator = GetComponent<Animator>();
        currentState = AIState.Patrol;
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;


        // Initialize walkPoint to the origin (Vector3.zero)
        walkPoint = Vector3.zero;
        playerHealth = player.GetComponent<PlayerHealth>();

        // Initialize the current health to the maximum health
        currentHealth = maxHealth;
        CheckIfTutorialCompleted();
    }


    // The state of the enemy's AI
    private enum AIState
    {
        Patrol,
        Chase,
        Attack,
        Dead
    }

    private AIState currentState;

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.Attack:
                Attack();
                break;
            case AIState.Dead:
                Dead();
                break;
        }
    }

    public void CheckIfTutorialCompleted()
    {
        if (SceneManager.GetActiveScene().name != tutorialSceneName)
        {
            tutorialIsCompleted = true;
            Debug.Log(tutorialIsCompleted);
        }

        if (tutorialIsCompleted) 
        {
            navMeshAgent.enabled = true;
            return;
        }
        else
        {
            navMeshAgent.enabled = false;
        }
    }

    public void SearchWalkPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkPointRange, 1))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
        else
        {
            walkPointSet = false;
        }
    }

    void Patrol()
    {
        if (!walkPointSet && tutorialIsCompleted)
        {
            SearchWalkPoint();

        }

        if (walkPointSet)
        {
            navMeshAgent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }

        // Check if the player is within the enemy's field of view
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle < fieldOfView * 0.5f)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hitInfo, detectionRange))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    currentState = AIState.Chase;
                }
            }
        }
        else
        {
            // Set the animation trigger for patrolling
            animator.SetTrigger(patrolTriggerHash);
        }
    }

    void Chase()
    {

        // Check if the player's position has changed
        if (direction != player.position - transform.position)
        {
            // Update the direction to the player
            direction = player.position - transform.position;
        }

        // Check if the enemy has seen the player before
        if (hasSeenPlayer)
        {
            // Calculate the distance between the enemy and the player
            float distance = Vector3.Distance(transform.position, player.position);

            // Set the destination for the enemy to move towards the player
            navMeshAgent.SetDestination(player.position);

            // Check if the player is within attack range
            if (distance <= attackRange)
            {
                currentState = AIState.Attack;
            }
            else
            {
                // Set the animation trigger for chasing the player
                animator.SetTrigger(chaseTriggerHash);
            }
        }
        else
        {
            // Use raycasting to detect if the player is in sight
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, detectionRange))
            {
                // Calculate the distance between the enemy and the player
                float distance = Vector3.Distance(transform.position, player.position);

                // Check if the player is within detection range
                if (distance <= breakfreeRange)
                {
                    // Set the destination for the enemy to move towards
                    navMeshAgent.SetDestination(player.position);

                    // Check if the player is within attack range
                    if (distance <= attackRange)
                    {
                        currentState = AIState.Attack;
                    }
                    else
                    {
                        // Set the animation trigger for chasing the player
                        animator.SetTrigger(chaseTriggerHash);
                    }
                }
                else
                {
                    // If the player is out of range, switch back to the Idle state
                    currentState = AIState.Patrol;
                }
            }
        }
    }

    void Attack()
    {
        // Calculate the direction to the player
        Vector3 direction = player.position - transform.position;

        // Rotate the enemy towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

        // Check if the player is within attack range
        if (direction.magnitude > attackRange)
        {
            // Switch to the Chase state if the player is no longer within range
            currentState = AIState.Chase;
        }
        else
        {
            // Stand Still
            navMeshAgent.SetDestination(transform.position);
            animator.SetTrigger(attackTriggerHash);
        }
        if (alreadyAttacked == true)
        {
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

    }

    void Dead()
    {
        transform.position = transform.position;
    }

    void DealDamage()
    {
        // Attack the player
        playerHealth.TakeDamage(damageTaken);
    }

    public void OnHitWithAxe(Collider other)
    {
        if (other.gameObject.CompareTag("Player")&& !alreadyAttacked)
        {             
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                DealDamage();
                alreadyAttacked = true;
            }
        }
    }


    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        // Decrease the current health by the damage taken
        currentHealth -= damage;
        // If the current health is less than or equal to 0, set the current health to 0 and call the Die method
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //play death animation
            animator.SetTrigger(dieTriggerHash);
            currentState = AIState.Dead;
            StartCoroutine("Die");
        }
    }

    public IEnumerator Die()
    {
        //destroy game object   
        yield return new WaitForSeconds(3);
        //check if all enemies are dead and open door if so
        GameObject doorOpener = GameObject.Find("DoorManager");
        doorOpener.GetComponent<DoorOpener>().OpenDoor();
        Destroy(gameObject);
    }
}