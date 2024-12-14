using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
//using System.Security.Cryptography.X509Certificates;
//using System.Security.Cryptography.X509Certificates;
using System.Collections;
using UnityEngine.Splines;
using UnityEngine.UI;  // For the Slider component


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
    public bool stopMoving;

    public ComboCounter comboC;
    public int triCombo;
    public int randCombo;
    public int currentFailedHit;
    public int maxFailedHits;
    private Vector3 direction;

    public int currentHealth;
    public int maxHealth;
    public Image healthBarImage;  // Reference to the UI health bar image


    // Knockback and Stun
    public float knockbackDistance = 2f;
    public float knockbackSpeed = 5f;
    public float stunDuration = 1f;
    private bool isKnockedBack = false;
    public GameObject playerMove;

    public bool playerImmune;
    public float time;
    public float immuneTime = 5f;

    public GameController gameC;

    void Start()
    {
        playerControls = new PlayerControls();
        currentHealth = maxHealth;
        // Initialize the health bar
        UpdateHealthBar();
    }

    void Update()
    {
        if (playerImmune)
        {
            time += Time.fixedDeltaTime;
            if (time >= immuneTime)
            {
                time = 0f;
                playerImmune = false;
            }
        }
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
            direction = (currentTarget.transform.position - transform.position).normalized;
            playerCont.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
        // Play hit animation only if desired
        if (isKnockedBack) // Adjust condition as needed
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            animator.ResetTrigger("Hit");
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
                    Transform parentTransform = hit.transform.parent;

                    if (parentTransform != null)
                    {
                        AIController parentScript = parentTransform.GetComponent<AIController>();
                        print("Bool set to true on parent object.");

                        if (parentScript != null)
                        {
                            parentScript.playerHit = true;

                            if (currentTarget != null)
                            {
                                direction = (currentTarget.transform.position - transform.position).normalized;
                            }
                            else
                            {
                                direction = (hit.transform.position - transform.position).normalized;
                            }

                            parentScript.TakeDamage(20, direction);
                            print("Bool set to true on parent object.");
                        }
                        else
                        {
                            Debug.LogWarning("AIController component is missing on parent: " + parentTransform.name);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Parent transform is null for object: " + hit.transform.name);
                    }

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

        if (!stopMoving)
        {
            if (currentPosition == targetPositionWithOffset)
            {
                //stopMoving = true;
                //stopMoving = true;
            }
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
        //ApplyDamage();
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
                stopMoving = false;
            }
        }

        // Reset counter state after animation
        if (isCountering && !animator.GetCurrentAnimatorStateInfo(0).IsName("Counter"))
        {
            isCountering = false;
            tsm.combat = false;
        }
    }

    // New Methods
    public void TakeDamage(int damage, Vector3 knockbackDirection)
    {
        if (!playerImmune)
        {
            print("TAKE DAMAMGE");
            if (currentHealth <= 0) return;

            if (!isKnockedBack)
            {
                currentHealth -= damage;
                print($"PLAYER took {damage} damage! Current health: {currentHealth}");
            }

            UpdateHealthBar();

            if (currentHealth > 0)
            {
                // Apply knockback and stun
                StartCoroutine(ApplyKnockback(knockbackDirection));
            }
            else
            {
                // Handle AI death
                Die();
            }
        }
        
    }

    // Method to update the health bar UI
    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            // Calculate the fill amount as a fraction of current health / max health
            healthBarImage.fillAmount = (float)currentHealth / (float)maxHealth;
        }
    }

    IEnumerator ApplyKnockback(Vector3 direction)
    {
        isKnockedBack = true;

        // Disable agent for knockback
        Vector3 startPosition = playerMove.transform.position;
        Vector3 targetPosition = startPosition + direction.normalized * knockbackDistance;

        float elapsedTime = 0f;
        float duration = knockbackDistance / knockbackSpeed;

        // Lerp to create smooth knockback
        while (elapsedTime < duration)
        {
            playerMove.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            Vector3 newPosition = playerMove.transform.position;
            newPosition.y = startPosition.y;
            playerMove.transform.position = newPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerMove.transform.position = targetPosition; // Ensure it ends exactly at the target

        yield return new WaitForSeconds(stunDuration);


        isKnockedBack = false;
    }


    void Die()
    {
        print("PLAYER has died!");

        // Play death animation
        animator.SetTrigger("Die");

        // Destroy object after death animation
        Destroy(gameObject, 3f); // Adjust delay based on animation length
        gameC.GameOver(comboC.highestCombo);
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


