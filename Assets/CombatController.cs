using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    // Configurable Fields
    public float attackRange = 2f;
    public float counterRange = 2f;
    public float attackDelay = 1.5f;
    public int baseDamage = 10;
    public float coneAngle = 45f;  // Angle of the cone for detection
    public int rayCount = 5;       // Number of rays in the cone

    // Animator
    public Animator animator;
    public TwinStickMovement tsm;

    // Enemy Tracking
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private GameObject currentTarget;

    // Attack State Management
    public bool isAttacking = false;
    private bool isCountering = false;
    private float attackTimer;

    // Layers
    public LayerMask enemyLayer; // Layer to specify which objects are considered enemies

    private PlayerControls playerControls;

    public GameObject playerCont;
    public float moveSpeed = 5f;  // Speed of movement
    public float rotateSpeed = 5f;  // Speed of rotation
    public float attackMoveOffset = 1.4f;

    // Attack Distance
    public float attackDistance;

    public ComboCounter comboC;
    public int triCombo;
    public int randCombo;
    public int currentFailedHit;
    public int maxFailedHits;

    void Start()
    {
        playerControls = new PlayerControls();
    }

    void Update()
    {
        ManageAttackCooldown();
        tsm.combat = isAttacking;
        // Calculate target rotation based on movement direction
        transform.rotation = tsm.transform.rotation;
        tsm.CombatAim(this.gameObject);

        if (isAttacking)
        {
            // move towards player
            //MoveTowardsTarget(playerCont, moveSpeed);
            MoveTowardsTargetWithStoppingDistance(playerCont.gameObject, currentTarget, attackMoveOffset, moveSpeed);
            // Rotate toward target
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            playerCont.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }

    }

    // Referenced in Player Input System
    // Handle basic attack input
    public void HandleAttackInput()
    {
        if (!isAttacking && !isCountering)
        {
            if (FindTargetWithinCone(attackRange, coneAngle))
            {
                PerformAttack();
                comboC.currentCombo++;
                currentFailedHit = 0;
            }
            else
            {
                currentFailedHit++;
                if (currentFailedHit > maxFailedHits)
                {
                    comboC.currentCombo = 0;
                    currentFailedHit = 0;
                    triCombo = 0;
                }
            }
        }
    }

    // Handle counter input
    public void HandleCounterInput()
    {
        if (!isAttacking)
        {
            PerformCounter();
        }
    }

    // Find closest target within a cone of rays
    bool FindTargetWithinCone(float range, float angle)
    {
        currentTarget = null;
        float closestDistance = Mathf.Infinity;

        // Calculate the angle step based on the number of rays
        float angleStep = angle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            // Calculate the direction for each ray in the cone
            float currentAngle = -angle / 2 + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            // Cast a ray in the calculated direction
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, range, enemyLayer))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    attackDistance = Vector3.Distance(transform.position, hit.point);

                    // Update the closest target within the cone
                    if (attackDistance < closestDistance)
                    {
                        closestDistance = attackDistance;
                        currentTarget = hit.collider.gameObject;
                    }
                }
            }
        }

        return currentTarget != null;
    }

    // Perform attack on the current target
    void PerformAttack()
    {
        print("attack");
        // Trigger the attack animation
        RandomiseAnimation();
        animator.SetTrigger("Attack");
        animator.SetFloat("AttackDistance", attackDistance);
        animator.SetInteger("RandCombo", randCombo);
        animator.SetInteger("triCombo", triCombo);
        tsm.combat = true;
        isAttacking = true;
        attackTimer = attackDelay;

        // Apply damage after animation delay
        Invoke("ApplyDamage", attackDelay);
    }

    void RandomiseAnimation()
    {
        print("rand muber");
        if (triCombo != 3)
        {
            print("combo up");
            triCombo += 1;
        }
        else
        {
            triCombo = 0;
        }

        if (attackDistance < 3)
        {
            if (triCombo == 0)
            {
                randCombo = UnityEngine.Random.Range(0, 5);
            }
            else if (triCombo == 1)
            {
                randCombo = UnityEngine.Random.Range(0, 6);
            }
            else if (triCombo == 2)
            {
                randCombo = UnityEngine.Random.Range(0, 4);
            }
        }
        else if (attackDistance > 3)
        {
            if (triCombo == 0)
            {
                randCombo = UnityEngine.Random.Range(0, 5);
            }
            else if (triCombo == 1)
            {
                randCombo = UnityEngine.Random.Range(0, 5);
            }
            else if (triCombo == 2)
            {
                randCombo = UnityEngine.Random.Range(0, 5);
            }
        }
    }

    void RotateTowardsTarget(GameObject gameObject)
    {
        // Get direction to the target
        Vector3 directionToTarget = currentTarget.transform.position - gameObject.transform.position;

        // Calculate the rotation needed to face the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Smoothly rotate towards the target rotation
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    void MoveTowardsTarget(GameObject gameObject, float speed)
    {
        // Move towards the target position
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, currentTarget.transform.position, speed * Time.deltaTime);
    }

    void MoveTowardsTargetWithStoppingDistance(GameObject gameObject, GameObject currentTarget, float stoppingDistance, float speed)
    {
        // Get the current position and target position
        Vector3 currentPosition = gameObject.transform.position;
        Vector3 targetPosition = currentTarget.transform.position;

        // Ignore Y-axis movement by setting Y values to the same
        currentPosition.y = 0f;
        targetPosition.y = 0f;

        // Calculate the direction to the target (ignoring Y)
        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;

        // Calculate the target position with the stopping distance offset (ignoring Y)
        Vector3 targetPositionWithOffset = targetPosition - directionToTarget * stoppingDistance;

        // Move towards the offset target position (ignoring Y)
        Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPositionWithOffset, speed * Time.deltaTime);

        // Preserve the original Y position of the game object
        newPosition.y = gameObject.transform.position.y;

        // Update the game object's position
        gameObject.transform.position = newPosition;

        // Optional: Ensure the object doesn't overshoot the stopping distance
        if (Vector3.Distance(new Vector3(currentPosition.x, 0, currentPosition.z), new Vector3(targetPositionWithOffset.x, 0, targetPositionWithOffset.z)) < 0.1f)
        {
            gameObject.transform.position = targetPositionWithOffset + new Vector3(0, gameObject.transform.position.y, 0);
        }
    }

    // Perform counter on the current target
    void PerformCounter()
    {
        tsm.combat = true;
        isCountering = true;

        // Trigger the counter animation
        animator.SetTrigger("Counter");

        // Rotate toward target
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Apply damage immediately (or adjust delay for counter animation timing)
        ApplyDamage();
    }

    // Manage attack cooldown to avoid spamming attacks
    void ManageAttackCooldown()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isAttacking = false;
                tsm.combat = false;
            }
        }

        // Reset counter state after animation
        if (isCountering && !animator.GetCurrentAnimatorStateInfo(0).IsName("Counter"))
        {
            isCountering = false;
            tsm.combat = false;
        }
    }

    // Apply damage to the target
    void ApplyDamage()
    {
        /*
        if (currentTarget != null)
        {
            // Assuming the enemy has a health script
            EnemyHealth enemyHealth = currentTarget.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(baseDamage);
            }
        }
        */
    }

    // Detect when enemies are within range
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    // Debugging - Draw a cone of rays to visualize the detection area
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Calculate the angle step
        float angleStep = coneAngle / (rayCount - 1);

        // Draw each ray in the cone
        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = -coneAngle / 2 + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Gizmos.DrawLine(transform.position, transform.position + rayDirection * attackRange);
        }
    }
}


