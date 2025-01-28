using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Array to store references to all enemies in the scene
    private EnemyScript[] enemies;

    // Array to track enemy scripts and their availability
    public EnemyStruct[] allEnemies;

    // List to store indexes of available enemies for selection
    private List<int> enemyIndexes;

    [Header("Main AI Loop - Settings")]
    // Coroutine responsible for managing the main AI loop
    private Coroutine AI_Loop_Coroutine;

    // Counter to track the number of alive enemies
    public int aliveEnemyCount;

    void Start()
    {
        // Fetch all EnemyScript components from child objects
        enemies = GetComponentsInChildren<EnemyScript>();

        // Initialize the allEnemies array with the same size as enemies
        allEnemies = new EnemyStruct[enemies.Length];

        // Populate allEnemies array with enemy references and set their availability to true
        for (int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].enemyScript = enemies[i];
            allEnemies[i].enemyAvailability = true;
        }

        // Start the main AI loop
        StartAI();
    }

    // Starts the main AI loop coroutine
    public void StartAI()
    {
        AI_Loop_Coroutine = StartCoroutine(AI_Loop(null));
    }

    // Main AI loop controlling enemy behavior
    IEnumerator AI_Loop(EnemyScript enemy)
    {
        // Exit the loop if there are no alive enemies
        if (AliveEnemyCount() == 0)
        {
            StopCoroutine(AI_Loop(null));
            yield break;
        }

        // Wait for a random delay before assigning the next action
        yield return new WaitForSeconds(Random.Range(.5f, 1.5f));

        // Select a random enemy to attack, excluding the one currently attacking
        EnemyScript attackingEnemy = RandomEnemyExcludingOne(enemy);

        // Fallback: if no enemy is found, pick any random enemy
        if (attackingEnemy == null)
            attackingEnemy = RandomEnemy();

        // If no enemy is available, exit the loop
        if (attackingEnemy == null)
            yield break;

        // Wait until the enemy is not retreating, locked, or stunned
        yield return new WaitUntil(() => !attackingEnemy.IsRetreating());
        yield return new WaitUntil(() => !attackingEnemy.IsLockedTarget());
        yield return new WaitUntil(() => !attackingEnemy.IsStunned());

        // Trigger the enemy to attack
        attackingEnemy.SetAttack();

        // Wait for the attack preparation to complete
        yield return new WaitUntil(() => !attackingEnemy.IsPreparingAttack());

        // Make the enemy retreat after attacking
        attackingEnemy.SetRetreat();

        // Add a small random delay before selecting the next enemy
        yield return new WaitForSeconds(Random.Range(0, .5f));

        // If there are still alive enemies, restart the AI loop for the next enemy
        if (AliveEnemyCount() > 0)
            AI_Loop_Coroutine = StartCoroutine(AI_Loop(attackingEnemy));
    }

    // Returns a random enemy from the available list
    public EnemyScript RandomEnemy()
    {
        enemyIndexes = new List<int>();

        // Add indexes of available enemies to the list
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability)
                enemyIndexes.Add(i);
        }

        // If no available enemies, return null
        if (enemyIndexes.Count == 0)
            return null;

        // Select a random enemy from the list
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        return allEnemies[enemyIndexes[randomIndex]].enemyScript;
    }

    // Returns a random enemy, excluding a specific enemy
    public EnemyScript RandomEnemyExcludingOne(EnemyScript exclude)
    {
        enemyIndexes = new List<int>();

        // Add indexes of available enemies that are not the excluded enemy
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability && allEnemies[i].enemyScript != exclude)
                enemyIndexes.Add(i);
        }

        // If no available enemies, return null
        if (enemyIndexes.Count == 0)
            return null;

        // Select a random enemy from the filtered list
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        return allEnemies[enemyIndexes[randomIndex]].enemyScript;
    }

    // Returns the number of available enemies
    public int AvailableEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability)
                count++;
        }
        return count;
    }

    // Checks if any enemy is currently preparing an attack
    public bool AnEnemyIsPreparingAttack()
    {
        foreach (EnemyStruct enemyStruct in allEnemies)
        {
            if (enemyStruct.enemyScript.IsPreparingAttack())
            {
                return true;
            }
        }
        return false;
    }

    // Returns the number of alive enemies
    public int AliveEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyScript.isActiveAndEnabled)
                count++;
        }
        aliveEnemyCount = count;
        return count;
    }

    // Sets the availability of a specific enemy
    public void SetEnemyAvailiability(EnemyScript enemy, bool state)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyScript == enemy)
                allEnemies[i].enemyAvailability = state;
        }

        // Reset the current target if it is the same as the enemy being modified
        if (FindObjectOfType<EnemyDetection>().CurrentTarget() == enemy)
            FindObjectOfType<EnemyDetection>().SetCurrentTarget(null);
    }
}

// Struct to hold enemy script and its availability
[System.Serializable]
public struct EnemyStruct
{
    public EnemyScript enemyScript; // Reference to the enemy script
    public bool enemyAvailability; // Whether the enemy is available for actions
}