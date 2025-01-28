using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Unity.Cinemachine;

// Core script that manages the player's combat behavior and interactions with enemies
public class CombatScript : MonoBehaviour
{
    // Component and manager references
    private EnemyManager enemyManager; // Manages all enemies in the scene
    private EnemyDetection enemyDetection; // Detects the player's target enemy
    private MovementInput movementInput; // Handles player movement
    private Animator animator; // Controls character animations
    private CinemachineImpulseSource impulseSource; // Creates camera shake effects

    [Header("Target")]
    private EnemyScript lockedTarget; // The currently locked-on enemy

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown; // Cooldown time between attacks

    [Header("States")]
    public bool isAttackingEnemy = false; // Is the player currently attacking?
    public bool isCountering = false; // Is the player countering an enemy's attack?

    [Header("Public References")]
    [SerializeField] private Transform punchPosition; // Position where punch VFX will play
    [SerializeField] private ParticleSystemScript punchParticle; // Script for handling punch particle effects
    [SerializeField] private GameObject lastHitCamera; // Camera for "last hit" cinematic
    [SerializeField] private Transform lastHitFocusObject; // Object to focus on during "last hit"

    // Coroutines
    private Coroutine counterCoroutine; // Tracks the countering coroutine
    private Coroutine attackCoroutine; // Tracks the attacking coroutine
    private Coroutine damageCoroutine; // Tracks the damage coroutine

    [Space]

    // Unity Events
    public UnityEvent<EnemyScript> OnTrajectory; // Event for moving toward an enemy
    public UnityEvent<EnemyScript> OnHit; // Event when an enemy is hit
    public UnityEvent<EnemyScript> OnCounterAttack; // Event when counter-attacking

    // Internal variables
    int animationCount = 0; // Counter for cycling attack animations
    string[] attacks; // Array of attack animation triggers

    void Start()
    {
        // Initialize references to components and managers
        enemyManager = FindObjectOfType<EnemyManager>();
        animator = GetComponent<Animator>();
        enemyDetection = GetComponentInChildren<EnemyDetection>();
        movementInput = GetComponent<MovementInput>();
        impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    // Checks whether an attack can be performed
    void AttackCheck()
    {
        if (isAttackingEnemy)
            return; // Exit if the player is already attacking

        // If no target is detected, pick a random enemy
        if (enemyDetection.CurrentTarget() == null)
        {
            if (enemyManager.AliveEnemyCount() == 0)
            {
                Attack(null, 0); // Attack without a target if no enemies exist
                return;
            }
            else
            {
                lockedTarget = enemyManager.RandomEnemy(); // Select a random enemy
            }
        }

        // If the player is moving, use input direction to detect the nearest enemy
        if (enemyDetection.InputMagnitude() > 0.2f)
            lockedTarget = enemyDetection.CurrentTarget();

        // As a fallback, pick a random enemy if no target is set
        if (lockedTarget == null)
            lockedTarget = enemyManager.RandomEnemy();

        // Proceed to attack the locked target
        Attack(lockedTarget, TargetDistance(lockedTarget));
    }

    // Performs an attack on a specified target
    public void Attack(EnemyScript target, float distance)
    {
        // Define the attack animations
        attacks = new string[] { "AirKick", "AirKick2", "AirPunch", "AirKick3" };

        // If no target exists, perform a "ground punch"
        if (target == null)
        {
            AttackType("GroundPunch", 0.2f, null, 0);
            return;
        }

        // If the target is within range, select an appropriate attack animation
        if (distance < 15)
        {
            animationCount = (int)Mathf.Repeat((float)animationCount + 1, attacks.Length);
            string attackString = isLastHit() ? attacks[Random.Range(0, attacks.Length)] : attacks[animationCount];
            AttackType(attackString, attackCooldown, target, 0.65f);
        }
        else
        {
            lockedTarget = null; // Clear the target if it's too far away
            AttackType("GroundPunch", 0.2f, null, 0);
        }

        // Apply a camera impulse effect based on distance
        impulseSource.GenerateImpulse(Mathf.Max(3, 1 * distance));
    }

    // Executes a specific attack animation and handles attack-related logic
    void AttackType(string attackTrigger, float cooldown, EnemyScript target, float movementDuration)
    {
        animator.SetTrigger(attackTrigger); // Trigger the attack animation

        // Stop any existing attack coroutine
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(isLastHit() ? 1.5f : cooldown));

        // If this is the last enemy, execute the final blow cinematic
        if (isLastHit())
            StartCoroutine(FinalBlowCoroutine());

        if (target == null)
            return;

        target.StopMoving(); // Stop the target's movement
        MoveTorwardsTarget(target, movementDuration); // Move towards the target

        // Coroutine for managing the attack state
        IEnumerator AttackCoroutine(float duration)
        {
            movementInput.acceleration = 0;
            isAttackingEnemy = true;
            movementInput.enabled = false;
            yield return new WaitForSeconds(duration);
            isAttackingEnemy = false;
            yield return new WaitForSeconds(0.2f);
            movementInput.enabled = true;
            LerpCharacterAcceleration(); // Smoothly restore acceleration
        }

        // Coroutine for the final cinematic blow
        IEnumerator FinalBlowCoroutine()
        {
            Time.timeScale = 0.5f; // Slow down time
            lastHitCamera.SetActive(true); // Activate the cinematic camera
            lastHitFocusObject.position = lockedTarget.transform.position;
            yield return new WaitForSecondsRealtime(2); // Wait in real-time
            lastHitCamera.SetActive(false);
            Time.timeScale = 1f; // Reset time scale
        }
    }

    // Moves the player toward the target enemy
    void MoveTorwardsTarget(EnemyScript target, float duration)
    {
        OnTrajectory.Invoke(target); // Trigger the trajectory event
        transform.DOLookAt(target.transform.position, 0.2f); // Rotate toward the target
        transform.DOMove(TargetOffset(target.transform), duration); // Move to the target's position
    }

    // Checks whether the player should counter an enemy's attack
    void CounterCheck()
    {
        if (isCountering || isAttackingEnemy || !enemyManager.AnEnemyIsPreparingAttack())
            return;

        lockedTarget = ClosestCounterEnemy(); // Get the closest enemy preparing an attack
        OnCounterAttack.Invoke(lockedTarget);

        if (TargetDistance(lockedTarget) > 2)
        {
            Attack(lockedTarget, TargetDistance(lockedTarget)); // Attack instead if the enemy is far
            return;
        }

        float duration = 0.2f;
        animator.SetTrigger("Dodge"); // Trigger the dodge animation
        transform.DOLookAt(lockedTarget.transform.position, 0.2f);
        transform.DOMove(transform.position + lockedTarget.transform.forward, duration);

        if (counterCoroutine != null)
            StopCoroutine(counterCoroutine);
        counterCoroutine = StartCoroutine(CounterCoroutine(duration));

        // Coroutine for managing counter state
        IEnumerator CounterCoroutine(float duration)
        {
            isCountering = true;
            movementInput.enabled = false;
            yield return new WaitForSeconds(duration);
            Attack(lockedTarget, TargetDistance(lockedTarget));
            isCountering = false;
        }
    }

    // Returns the distance between the player and the target
    float TargetDistance(EnemyScript target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    // Offsets the target's position to prevent direct overlap
    public Vector3 TargetOffset(Transform target)
    {
        Vector3 position = target.position;
        return Vector3.MoveTowards(position, transform.position, 0.95f);
    }

    // Event triggered when the player hits an enemy
    public void HitEvent()
    {
        if (lockedTarget == null || enemyManager.AliveEnemyCount() == 0)
            return;

        OnHit.Invoke(lockedTarget);
        punchParticle.PlayParticleAtPosition(punchPosition.position); // Play punch effects
    }

    // Event triggered when the player takes damage
    public void DamageEvent()
    {
        animator.SetTrigger("Hit"); // Trigger the hit animation

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        damageCoroutine = StartCoroutine(DamageCoroutine());

        // Coroutine for managing damage state
        IEnumerator DamageCoroutine()
        {
            movementInput.enabled = false;
            yield return new WaitForSeconds(0.5f);
            movementInput.enabled = true;
            LerpCharacterAcceleration();
        }
    }

    // Finds the closest enemy that is preparing to attack
    EnemyScript ClosestCounterEnemy()
    {
        float minDistance = 100;
        int finalIndex = 0;

        for (int i = 0; i < enemyManager.allEnemies.Length; i++)
        {
            EnemyScript enemy = enemyManager.allEnemies[i].enemyScript;

            if (enemy.IsPreparingAttack() && Vector3.Distance(transform.position, enemy.transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(transform.position, enemy.transform.position);
                finalIndex = i;
            }
        }

        return enemyManager.allEnemies[finalIndex].enemyScript;
    }

    // Smoothly restores the player's acceleration
    void LerpCharacterAcceleration()
    {
        movementInput.acceleration = 0;
        DOVirtual.Float(0, 1, 0.6f, (acceleration) => movementInput.acceleration = acceleration);
    }

    // Checks if the current attack is the "last hit"
    bool isLastHit()
    {
        return lockedTarget != null && enemyManager.AliveEnemyCount() == 1 && lockedTarget.health <= 1;
    }

    #region Input

    private void OnCounter()
    {
        CounterCheck(); // Handle counter input
    }

    private void OnAttack()
    {
        AttackCheck(); // Handle attack input
    }

    #endregion
}
