using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI; // Required for NavMesh
using System.Collections;

public class AIController : MonoBehaviour
{
    // Core AI Components
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    // AI Stats
    public float followDistance = 10f;   // Start following within this range
    public float attackDistance = 2f;   // Attack when within this range
    public float attackCooldown = 1.5f; // Time between attacks
    public int maxHealth = 100;         // AI's max health
    private int currentHealth;

    // Attack and Health System
    private bool isAttacking = false;
    private bool isKnockedBack = false;
    public float knockbackForce = 5f;
    public float stunDuration = 1f;

    // VFX and SFX
    public GameObject deathVFX; // VFX prefab for death
    public AudioClip attackSound; // Sound to play during attack
    public AudioClip hitSound; // Sound to play when hit

    private float attackTimer = 0f;

    void Start()
    {
        // Get required components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Set current health
        currentHealth = maxHealth;

        // Find the player if not assigned
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player == null)
        {
            print("Player not found. Ensure your player is tagged as 'Player' or assigned manually.");
        }
    }

    void Update()
    {
        if (isKnockedBack || currentHealth <= 0)
            return; // Prevent movement when stunned or dead

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= followDistance)
        {
            if (distanceToPlayer > attackDistance)
            {
                // Move toward the player
                agent.SetDestination(player.position);
                animator.SetBool("isWalking", true); // Play walking animation
            }
            else
            {
                // Attack the player
                animator.SetBool("isWalking", false); // Stop walking
                if (!isAttacking && attackTimer <= 0f)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
        }
        else
        {
            // Stop movement and animations
            agent.ResetPath();
            animator.SetBool("isWalking", false);
        }

        // Update attack cooldown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        // Play attack animation and sound
        animator.SetTrigger("Attack");
        if (attackSound) AudioSource.PlayClipAtPoint(attackSound, transform.position);

        // Simulate damage to player here
        print("Player attacked!");

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void TakeDamage(int damage, Vector3 knockbackDirection)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        print($"AI took {damage} damage! Current health: {currentHealth}");

        if (hitSound) AudioSource.PlayClipAtPoint(hitSound, transform.position);

        if (currentHealth > 0)
        {
            // Apply knockback and stun
            StartCoroutine(ApplyKnockback(knockbackDirection));
        }
        else
        {
            // Play death logic
            Die();
        }
    }

    IEnumerator ApplyKnockback(Vector3 direction)
    {
        isKnockedBack = true;

        // Play hit animation
        animator.SetTrigger("Hit");

        // Apply knockback force
        agent.enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(stunDuration);

        // Re-enable NavMeshAgent after stun
        if (rb) rb.linearVelocity = Vector3.zero;
        agent.enabled = true;

        isKnockedBack = false;
    }

    void Die()
    {
        print("AI has died!");

        // Stop all AI movement
        agent.ResetPath();
        agent.enabled = false;

        // Play death animation
        animator.SetTrigger("Die");

        // Instantiate VFX
        if (deathVFX)
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        }

        // Disable the AI after animation ends
        Destroy(gameObject, 3f); // Adjust delay based on animation length
    }
}
