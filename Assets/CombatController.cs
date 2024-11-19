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
    private TwinStickMovement tsm;

    // Enemy Tracking
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private GameObject currentTarget;

    // Attack State Management
    private bool isAttacking = false;
    private bool isCountering = false;
    private float attackTimer;

    // Layers
    public LayerMask enemyLayer; // Layer to specify which objects are considered enemies

    private PlayerControls playerControls;

    void Start()
    {
        tsm = GetComponent<TwinStickMovement>();
        playerControls = new PlayerControls();
    }

    void Update()
    {
        ManageAttackCooldown();
        tsm.combat = isAttacking;
    }

    // Handle basic attack input
    public void HandleAttackInput()
    {
        if (!isAttacking && !isCountering)
        {
            print("FireButton1");
            if (FindTargetWithinCone(attackRange, coneAngle))
            {
                PerformAttack();
            }
        }
    }

    // Handle counter input
    public void HandleCounterInput()
    {
        if (!isAttacking)
        {
            print("FireButton2");
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
                    float distance = Vector3.Distance(transform.position, hit.point);

                    // Update the closest target within the cone
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
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
        tsm.combat = true;
        print("Attack");
        isAttacking = true;
        attackTimer = attackDelay;

        // Trigger the attack animation
        animator.SetTrigger("Attack");

        // Rotate toward target
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Apply damage after animation delay
        Invoke("ApplyDamage", attackDelay);
    }

    // Perform counter on the current target
    void PerformCounter()
    {
        tsm.combat = true;
        print("Counter");
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


