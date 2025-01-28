using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class EnemyScript : MonoBehaviour
{
    // Components and Dependencies
    private Animator animator; // Animator to handle enemy animations
    private CombatScript playerCombat; // Reference to the player's combat system
    private EnemyManager enemyManager; // Manager to track enemy availability and states
    private EnemyDetection enemyDetection; // Detects the player's position and actions
    private CharacterController characterController; // For handling movement

    [Header("Stats")]
    public int health = 3; // Enemy health
    private float moveSpeed = 1; // Movement speed
    private Vector3 moveDirection; // Current movement direction

    [Header("States")]
    [SerializeField] private bool isPreparingAttack; // Indicates if the enemy is preparing an attack
    [SerializeField] private bool isMoving; // Indicates if the enemy is moving
    [SerializeField] private bool isRetreating; // Indicates if the enemy is retreating
    [SerializeField] private bool isLockedTarget; // Indicates if the enemy is locked onto a target
    [SerializeField] private bool isStunned; // Indicates if the enemy is stunned
    [SerializeField] private bool isWaiting = true; // Indicates if the enemy is idle/waiting

    [Header("Polish")]
    [SerializeField] private ParticleSystem counterParticle; // Particle effect for counter attacks

    // Coroutines
    private Coroutine PrepareAttackCoroutine;
    private Coroutine RetreatCoroutine;
    private Coroutine DamageCoroutine;
    private Coroutine MovementCoroutine;

    // Events
    public UnityEvent<EnemyScript> OnDamage; // Triggered when the enemy takes damage
    public UnityEvent<EnemyScript> OnStopMoving; // Triggered when the enemy stops moving
    public UnityEvent<EnemyScript> OnRetreat; // Triggered when the enemy retreats

    void Start()
    {
        // Initialize dependencies and set up event listeners
        enemyManager = GetComponentInParent<EnemyManager>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        playerCombat = FindObjectOfType<CombatScript>();
        enemyDetection = playerCombat.GetComponentInChildren<EnemyDetection>();

        // Listen to player's combat-related events
        playerCombat.OnHit.AddListener((x) => OnPlayerHit(x));
        playerCombat.OnCounterAttack.AddListener((x) => OnPlayerCounter(x));
        playerCombat.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));

        // Start the enemy movement coroutine
        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    IEnumerator EnemyMovement()
    {
        // Wait until the enemy is idle/waiting
        yield return new WaitUntil(() => isWaiting == true);

        // Randomly decide whether to move or stop
        int randomChance = Random.Range(0, 2);
        if (randomChance == 1)
        {
            int randomDir = Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left; // Randomly choose left or right
            isMoving = true;
        }
        else
        {
            StopMoving();
        }

        yield return new WaitForSeconds(1); // Wait for 1 second before deciding again
        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    void Update()
    {
        // Constantly face the player
        transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));

        // Move the enemy if a direction is set
        MoveEnemy(moveDirection);
    }

    // Triggered when the player hits the enemy
    void OnPlayerHit(EnemyScript target)
    {
        if (target == this)
        {
            StopEnemyCoroutines(); // Stop all current enemy actions
            DamageCoroutine = StartCoroutine(HitCoroutine()); // Handle being hit

            enemyDetection.SetCurrentTarget(null); // Reset the current target
            isLockedTarget = false;
            OnDamage.Invoke(this); // Notify listeners of damage

            health--; // Reduce health

            if (health <= 0)
            {
                Death(); // Handle death if health is zero
                return;
            }

            animator.SetTrigger("Hit"); // Trigger hit animation
            transform.DOMove(transform.position - (transform.forward / 2), .3f).SetDelay(.1f); // Small knockback effect

            StopMoving(); // Stop moving when hit
        }

        IEnumerator HitCoroutine()
        {
            isStunned = true; // Temporarily stun the enemy
            yield return new WaitForSeconds(.5f); // Stun duration
            isStunned = false; // Recover from stun
        }
    }

    // Triggered when the player performs a counterattack
    void OnPlayerCounter(EnemyScript target)
    {
        if (target == this)
        {
            PrepareAttack(false); // Cancel attack preparation
        }
    }

    // Triggered when the player sets a trajectory toward the enemy
    void OnPlayerTrajectory(EnemyScript target)
    {
        if (target == this)
        {
            StopEnemyCoroutines(); // Stop all enemy actions
            isLockedTarget = true; // Lock onto the target
            PrepareAttack(false); // Cancel any attack preparation
            StopMoving(); // Stop moving
        }
    }

    // Handles enemy death
    void Death()
    {
        StopEnemyCoroutines(); // Stop all actions
        this.enabled = false; // Disable the script
        characterController.enabled = false; // Disable movement
        animator.SetTrigger("Death"); // Play death animation
        enemyManager.SetEnemyAvailiability(this, false); // Notify the manager that this enemy is no longer active
    }

    // Initiates retreat behavior
    public void SetRetreat()
    {
        StopEnemyCoroutines(); // Stop all actions
        RetreatCoroutine = StartCoroutine(PrepRetreat()); // Begin retreat preparation

        IEnumerator PrepRetreat()
        {
            yield return new WaitForSeconds(1.4f); // Wait before retreating
            OnRetreat.Invoke(this); // Notify listeners of retreat
            isRetreating = true;
            moveDirection = -Vector3.forward; // Move backward
            isMoving = true;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, playerCombat.transform.position) > 4); // Retreat until far enough
            isRetreating = false;
            StopMoving(); // Stop retreating

            isWaiting = true; // Mark enemy as idle
            MovementCoroutine = StartCoroutine(EnemyMovement()); // Resume movement coroutine
        }
    }

    // Initiates attack behavior
    public void SetAttack()
    {
        isWaiting = false; // Mark enemy as active
        PrepareAttackCoroutine = StartCoroutine(PrepAttack()); // Begin attack preparation

        IEnumerator PrepAttack()
        {
            PrepareAttack(true); // Start attack preparation
            yield return new WaitForSeconds(.2f); // Delay before moving forward
            moveDirection = Vector3.forward; // Move toward the player
            isMoving = true;
        }
    }

    // Handles attack preparation logic
    void PrepareAttack(bool active)
    {
        isPreparingAttack = active;

        if (active)
        {
            counterParticle.Play(); // Play counter particle effect
        }
        else
        {
            StopMoving(); // Stop moving if not preparing to attack
            counterParticle.Clear(); // Clear and stop particle effects
            counterParticle.Stop();
        }
    }

    // Handles enemy movement based on direction
    void MoveEnemy(Vector3 direction)
    {
        // Adjust speed based on direction
        moveSpeed = direction == Vector3.forward ? 5 : (direction == -Vector3.forward ? 2 : 1);

        // Update animator parameters
        animator.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude * direction.z) / (5 / moveSpeed), .2f, Time.deltaTime);
        animator.SetBool("Strafe", direction == Vector3.right || direction == Vector3.left);
        animator.SetFloat("StrafeDirection", direction.normalized.x, .2f, Time.deltaTime);

        if (!isMoving) return; // Exit if not moving

        // Calculate movement direction
        Vector3 dir = (playerCombat.transform.position - transform.position).normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir; // Perpendicular direction
        Vector3 finalDirection = direction == Vector3.forward ? dir :
                                 (direction == Vector3.right || direction == Vector3.left ? (pDir * direction.normalized.x) :
                                 -transform.forward);

        if (direction == Vector3.right || direction == Vector3.left)
            moveSpeed /= 1.5f; // Reduce speed for strafing

        Vector3 movedir = finalDirection * moveSpeed * Time.deltaTime;
        characterController.Move(movedir); // Move the character

        if (isPreparingAttack && Vector3.Distance(transform.position, playerCombat.transform.position) < 2)
        {
            StopMoving();
            if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
                Attack(); // Attack if close enough
            else
                PrepareAttack(false); // Cancel attack if the player is countering
        }
    }

    // Executes the attack animation and movement
    private void Attack()
    {
        transform.DOMove(transform.position + (transform.forward / 1), .5f); // Move slightly forward
        animator.SetTrigger("AirPunch"); // Trigger punch animation
    }

    // Triggered when the enemy hits the player
    public void HitEvent()
    {
        if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
            playerCombat.DamageEvent(); // Deal damage to the player

        PrepareAttack(false); // Reset attack state
    }

    // Stops enemy movement
    public void StopMoving()
    {
        isMoving = false; // Disable movement
        moveDirection = Vector3.zero; // Reset direction
        if (characterController.enabled)
            characterController.Move(moveDirection); // Stop movement immediately
    }

    // Stops all active enemy coroutines
    void StopEnemyCoroutines()
    {
        PrepareAttack(false);

        if (isRetreating && RetreatCoroutine != null)
            StopCoroutine(RetreatCoroutine);

        if (PrepareAttackCoroutine != null)
            StopCoroutine(PrepareAttackCoroutine);

        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);

        if (MovementCoroutine != null)
            StopCoroutine(MovementCoroutine);
    }

    #region Public Booleans

    // Public helper methods to check enemy states
    public bool IsAttackable() => health > 0;
    public bool IsPreparingAttack() => isPreparingAttack;
    public bool IsRetreating() => isRetreating;
    public bool IsLockedTarget() => isLockedTarget;
    public bool IsStunned() => isStunned;

    #endregion
}
