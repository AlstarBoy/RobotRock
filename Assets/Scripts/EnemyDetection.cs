using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager; // Reference to the enemy manager to manage enemies
    private MovementInput movementInput; // Reference to the player's movement input script
    private CombatScript combatScript; // Reference to the player's combat script

    public LayerMask layerMask; // Layer mask to filter specific layers during sphere cast (e.g., detecting enemies)

    [SerializeField] Vector3 inputDirection; // The direction of player input (calculated based on camera and player movement)
    [SerializeField] private EnemyScript currentTarget; // The currently detected enemy target

    public GameObject cam; // Reference to the player's camera

    private void Start()
    {
        // Initialize references to the parent object's movement and combat scripts
        movementInput = GetComponentInParent<MovementInput>();
        combatScript = GetComponentInParent<CombatScript>();
    }

    private void Update()
    {
        OnDrawGizmos();

        // Get the main camera and calculate forward and right directions
        var camera = Camera.main;
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        // Ignore vertical components (y-axis) of forward and right directions
        forward.y = 0f;
        right.y = 0f;

        // Normalize the directions to ensure consistent magnitude
        forward.Normalize();
        right.Normalize();

        // Calculate input direction based on player's movement input and camera orientation
        inputDirection = forward * movementInput.moveAxis.y + right * movementInput.moveAxis.x;
        inputDirection = inputDirection.normalized; // Normalize to prevent varying magnitudes

        // Perform a sphere cast to detect enemies in the direction of the input
        RaycastHit info;
        if (Physics.SphereCast(transform.position, 3f, inputDirection, out info, 10, layerMask))
        {
            // Check if the detected object has an EnemyScript and is attackable
            if (info.collider.transform.GetComponent<EnemyScript>().IsAttackable())
                currentTarget = info.collider.transform.GetComponent<EnemyScript>();
        }
    }

    // Returns the currently detected enemy target
    public EnemyScript CurrentTarget()
    {
        return currentTarget;
    }

    // Sets a new current target manually (used to override detection logic)
    public void SetCurrentTarget(EnemyScript target)
    {
        currentTarget = target;
    }

    // Returns the magnitude of the player's input direction
    public float InputMagnitude()
    {
        return inputDirection.magnitude;
    }

    private void OnDrawGizmos()
    {
        // Visualize detection logic in the editor
        Gizmos.color = Color.black;

        // Draw a ray in the direction of input
        Gizmos.DrawRay(transform.position, inputDirection);

        // Draw a sphere around the player for the detection radius
        Gizmos.DrawWireSphere(transform.position, 1);

        // Highlight the current target if it exists
        if (CurrentTarget() != null)
            Gizmos.DrawSphere(CurrentTarget().transform.position, .5f);
    }
}
