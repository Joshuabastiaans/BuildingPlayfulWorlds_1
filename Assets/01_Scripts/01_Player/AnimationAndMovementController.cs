using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class AnimationAndMovementController : MonoBehaviour
{
    //declare reference variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    //store hashes or somethn like that
    int isWalkingHash;
    int isRunningHash;
    int isCrouchingHash;
    int isArmedHash;
    int isAttackingHash;


    //variable to store input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 currentCrouchMovement;
    private bool isMovementPressed;
    private bool isRunPressed;
    private bool isCrouchPressed;
    private bool isArmedPressed;
    private bool isAttackPressed;
    float rotationFactorPerFrame = 5.0f;
    float walkMultiplier = 0.4f;
    float runMultiplier = 2f;
    float crouchMuliplier = 0.08f;

    // The range of the attack
    public float attackRange = 1f;
    // The attack damage the player deals to enemies
    public int attackDamage = (int)50f;
    // The maximum health of the enemy
    public int maxHealth = 100;
    // The current health of the enemy
    private int currentHealth;

    // declare weapon collider
    public Collider weaponCollider;

    // declare audiomanager
    public AudioManager audioManager;

    private bool isArmedNow;
    private bool WeaponIsArmed;
    private bool isAttacking;
    private float attackTimer;

    public GameObject hand;
    public GameObject spine;
    public GameObject weapon;


    // The duration of the attack animation
    public float attackAnimationDuration = 2.0f;
    // The timer for the attack animation
    private float attackAnimationTimer = 2.0f;
    // A flag to track whether the attack has been executed or not
    private bool attackExecuted;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    Vector3 mousePos;


    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isArmedHash = Animator.StringToHash("isArmed");
        isAttackingHash = Animator.StringToHash("isAttacking");

        playerInput.CharacterControls.Walk.started += onMovementInput;
        playerInput.CharacterControls.Walk.canceled += onMovementInput;
        playerInput.CharacterControls.Walk.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        playerInput.CharacterControls.Crouch.started += onCrouch;
        playerInput.CharacterControls.Crouch.canceled += onCrouch;
        playerInput.CharacterControls.Armed.started += onArmed;
        playerInput.CharacterControls.Armed.canceled += onArmed;
        playerInput.CharacterControls.Attack.started += onAttack;
        playerInput.CharacterControls.Attack.canceled += onAttack;

        // Initialize the current health to the maximum health
        currentHealth = maxHealth;

        // Get a reference to the audio manager
        audioManager = FindObjectOfType<AudioManager>();

        // Assign the collider component to the weaponCollider variable
        weaponCollider = GetComponent<Collider>();

        Armed();
    }

    void onCrouch(InputAction.CallbackContext context)
    {
        isCrouchPressed = context.ReadValueAsButton();
    }

    void onArmed(InputAction.CallbackContext context)
    {
        isArmedPressed = context.ReadValueAsButton();
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void onAttack(InputAction.CallbackContext context)
    {
        isAttackPressed = context.ReadValueAsButton();
    }


    void onMovementInput(InputAction.CallbackContext context)
    {
        Quaternion rotationOffset = Quaternion.identity;
        rotationOffset.eulerAngles = new Vector2(0, 45);

        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * walkMultiplier + currentMovementInput.y * walkMultiplier;
        currentMovement.z = -currentMovementInput.x * walkMultiplier + currentMovementInput.y * walkMultiplier;
        currentRunMovement.x = currentMovementInput.x * runMultiplier + currentMovementInput.y * runMultiplier;
        currentRunMovement.z = -currentMovementInput.x * runMultiplier + currentMovementInput.y * runMultiplier;
        currentCrouchMovement.x = currentMovementInput.x * crouchMuliplier + currentMovementInput.y * crouchMuliplier;
        currentCrouchMovement.z = -currentMovementInput.x * crouchMuliplier + currentMovementInput.y * crouchMuliplier;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void Attack()
    {
        isArmedNow = true;
        Armed();
        isAttacking = true;
        attackTimer = 2.0f;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            mousePos = raycastHit.point;
        }
        // Make the player look at the mouse position
        transform.LookAt(mousePos, Vector3.up);

        // Get the player's rotation as a Quaternion
        Quaternion playerRotation = transform.rotation;

        // Extract the Y-axis rotation from the player's rotation
        float yRotation = playerRotation.eulerAngles.y;

        // Set the player's rotation to the Y-axis rotation, with zero rotation around the other axes
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (attackExecuted == false && isAttacking)
        {
            // Check if there are any enemies within range
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                // Deal damage to the enemy
                enemy.GetComponent<EnemyAI>().TakeDamage(attackDamage);
                attackExecuted = true;
                attackTimer = 2.0f;
            }
        }
    }

    void ResetAttack()
    {
        // If the attack has been executed, update the attack animation timer
        if (attackExecuted)
        {
            attackAnimationTimer -= Time.deltaTime;
            if (attackAnimationTimer <= 0.0f)
            {
                attackExecuted = false;
            }
        }
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }
    }

    void Armed()
    {
        isArmedNow = true;
        if (!WeaponIsArmed)
        {
            SetWeaponPositionArmed();
        }
        // Start a coroutine to disable isArmedNow after 5 seconds
        StartCoroutine(DisableArmed());
    }



    //will disable being armed after 10 seconds of being armed or attacking
    IEnumerator DisableArmed()
    {
        // Keep looping until isArmedNow is false
        while (true)
        {
            // Check if the weapon is still armed
            if (isArmedNow)
            {
                // The weapon is still armed, wait for 10 seconds or until isArmedNow becomes false
                float timer = 20f;
                while (timer > 0f && isArmedNow)
                {
                    yield return null;
                    timer -= Time.deltaTime;
                }

                // Check if the weapon is still armed after waiting
                if (isArmedNow)
                {
                    // The weapon is still armed, set isArmedNow to false
                    isArmedNow = false;
                    SetWeaponPositionUnarmed();
                }
            }
            else
            {
                // The weapon is not armed, wait for isArmedNow to become true again
                while (!isArmedNow)
                {
                    yield return null;
                }
            }
        }
    }

    void SetWeaponPositionArmed()
    {
        WeaponIsArmed = true;
        // Set the weapon's parent to the hand GameObject
        weapon.transform.parent = hand.transform;

        // Set the weapon's local position and rotation to match the position and orientation of the weapon in the hand
        weapon.transform.localPosition = new Vector3(-0.215f, 0.13f, 0.02f);
        weapon.transform.localRotation = Quaternion.Euler(90f, 0f, 180f);
    }

    void SetWeaponPositionUnarmed()
    {
        WeaponIsArmed = false;
        // Set the weapon's parent to the spine GameObject
        weapon.transform.parent = spine.transform;

        // Set the weapon's local position and rotation to match the position and orientation of the weapon on the back of the player
        weapon.transform.localPosition = new Vector3(0.05f, 0.2f, -0.13f);
        weapon.transform.localRotation = Quaternion.Euler(-14.5f, -90f, 250f);
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(currentMovement * Time.deltaTime);
        handleRotation();
        handleAnimation();
        HandleInput();
        ResetAttack();
    }

    void HandleInput()
    {
        if (isAttackPressed)
        {
            Attack();
        }

        if (isArmedPressed)
        {
            Armed();
        }

        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }

        if (isCrouchPressed)
        {
            characterController.Move(currentCrouchMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isCrouching = animator.GetBool(isCrouchingHash);
        bool isArmed = animator.GetBool(isArmedHash);
        bool isAttacking = animator.GetBool(isAttackingHash);

        if (isArmedNow)
        {
            isArmed = true;
        }
        else
        {
            isArmed = false;
        }

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }

        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        if ((!isArmedPressed && isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isArmedHash, false);
        }
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

        if ((isCrouchPressed) && !isCrouching)
        {
            animator.SetBool(isCrouchingHash, true);
        }
        else if ((!isCrouchPressed) && isCrouching)
        {
            animator.SetBool(isCrouchingHash, false);
        }
        if (isArmed)
        {
            animator.SetBool(isArmedHash, true);
            animator.SetBool(isCrouchingHash, false);
        }
        else if (!isArmed)
        {
            animator.SetBool(isArmedHash, false);
        }
        if ((isAttackPressed) && !isAttacking)
        {
            animator.SetBool(isAttackingHash, true);
        }
        else if ((!isAttackPressed) && isAttacking)
        {
            animator.SetBool(isAttackingHash, false);
        }
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void HandleSound()
    {
        // Get the current animation state
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // If the character is walking, play the footstep sound
        if (state.fullPathHash == isWalkingHash)
        {
            //play Walking sound
            audioManager.Play("Walking");
        }
        // If the character is running, play the dash sound
        else if (state.fullPathHash == isRunningHash)
        {
            //play Running sound
            audioManager.Play("Running");
        }
        // If the character is attacking, play the attack sound
        else if (state.fullPathHash == isAttackingHash)
        {
            //play Attack sound
            audioManager.Play("Attack");
        }
    }
}
