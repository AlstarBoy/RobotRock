using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinStickC : MonoBehaviour
{
    public float moveSpeed = 5f;       // Movement speed of the player
    public float rotationSpeed = 720f; // Rotation speed to smooth out aiming

    private Rigidbody rb;              // Reference to Rigidbody component for 3D physics
    private Vector3 moveInput;         // Stores movement input from the left stick
    public Vector3 aimInput;          // Stores aim input from the right stick or mouse

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get movement input from the left stick (or WASD/arrow keys)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveInput = new Vector3(moveX, 0, moveZ).normalized;

        // Get aiming input from the right stick (or mouse position)
        float aimX = Input.GetAxis("AimHorizontal");
        float aimZ = Input.GetAxis("AimVertical");
        aimInput = new Vector3(aimX, 0, aimZ);

        // If using a mouse for aiming, calculate aim direction towards mouse position
        if (aimInput == Vector3.zero)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 mousePosition = hit.point;
                aimInput = (mousePosition - transform.position).normalized;
                aimInput.y = 0; // Flatten the aim direction on the XZ plane
            }
        }
    }

    void FixedUpdate()
    {
        // Move the player in the direction of the move input
        Vector3 moveVelocity = moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z); // Preserve y for gravity

        // Only rotate if there is input for aiming
        if (aimInput != Vector3.zero)
        {
            // Calculate the target angle based on the aim input
            Quaternion targetRotation = Quaternion.LookRotation(aimInput, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
