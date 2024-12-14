using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Required for NavMesh

public class AIController : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    // AI Stats
    public float followDistance = 10f;   // Start following within this range
    public int attackDamage;
    public float attackDistance = 2f;   // Attack when within this range
    public float attackCooldown = 1.5f; // Time between attacks
    public float stopDistance = 1.5f;   // Distance to stop before colliding with the player
    public int maxHealth = 100;         // AI's max health
    public int currentHealth;
    private bool isAttacking = false;
    public int triCombo;
    public int randCombo;
    public bool playerHit;

    // Knockback and Stun
    public float knockbackDistance = 2f;
    public float knockbackSpeed = 5f;
    public float stunDuration = 1f;
    private bool isKnockedBack = false;
    public GameObject playerObject;
    private Vector3 directionToPlayer;

    void Start()
    {
        // Get required components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Set initial health
        currentHealth = maxHealth;
        playerObject = GameObject.FindGameObjectWithTag("Player");

        // Find the player if not assigned
        if (player == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player == null)
        {
            print("Player not found. Ensure your player is tagged as 'Player' or assigned manually.");
        }

        // Set stopping distance to prevent collision
        agent.stoppingDistance = stopDistance;
    }

    void Update()
    {
        if (currentHealth <= 0)
            return; // Prevent any action if AI is dead

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (distanceToPlayer <= followDistance && !isKnockedBack)
        {
            if (distanceToPlayer > attackDistance)
            {
                if (agent.enabled)
                {
                    // Move toward the player
                    agent.SetDestination(player.position);
                }
                animator.SetFloat("Vertical", 1);
                animator.SetBool("isWalking", true); // Play walking animation
            }
            else
            {
                if (agent.enabled)
                {
                    // Stop moving and attack
                    agent.ResetPath(); // Ensure the agent stops moving
                }
                animator.SetBool("isWalking", false);
                animator.SetFloat("Vertical", 0);

                if (!isAttacking)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
        }
        else
        {
            if (agent.enabled)
            {
                // Stop movement and animations when out of follow range
                agent.ResetPath();
            }
            animator.SetBool("isWalking", false);
        }

        if (isAttacking)
        {
            // Face the player before attacking
            directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
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

    IEnumerator AttackPlayer()
    {
        CombatController combatC = playerObject.GetComponent<CombatController>();
        
        if (!combatC.playerImmune)
        {
            isAttacking = true;

            // Play attack animation
            RandomiseAnimation();
            animator.SetFloat("AttackDistance", attackDistance);
            animator.SetInteger("RandCombo", randCombo);
            animator.SetInteger("triCombo", triCombo);
            animator.SetTrigger("Attack");
            animator.SetFloat("AttackDistance", attackDistance);
            combatC.TakeDamage(attackDamage + (triCombo * 2), directionToPlayer);
            combatC.playerImmune = true;


            // Simulate damage to player here
            print("Player attacked!");

            yield return new WaitForSeconds(attackCooldown);

            isAttacking = false;
            animator.SetTrigger("idle");
        }


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

    // New Methods
    public void TakeDamage(int damage, Vector3 knockbackDirection)
    {
        if (currentHealth <= 0) return;

        if (!isKnockedBack)
        {
            currentHealth -= damage;
            print($"AI took {damage} damage! Current health: {currentHealth}");
        }

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

    IEnumerator ApplyKnockback(Vector3 direction)
    {
        isKnockedBack = true;

        // Disable agent for knockback
        agent.enabled = false;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction.normalized * knockbackDistance;

        float elapsedTime = 0f;
        float duration = knockbackDistance / knockbackSpeed;

        // Lerp to create smooth knockback
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure it ends exactly at the target

        yield return new WaitForSeconds(stunDuration);

        // Re-enable NavMeshAgent after stun
        agent.enabled = true;

        isKnockedBack = false;
    }


    void Die()
    {
        print("AI has died!");

        // Stop movement
        agent.ResetPath();
        agent.enabled = false;

        // Play death animation
        animator.SetTrigger("Die");

        // Destroy object after death animation
        Destroy(gameObject, 3f); // Adjust delay based on animation length
    }
}
