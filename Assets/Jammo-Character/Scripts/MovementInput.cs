using UnityEngine;
using UnityEngine.InputSystem;

// Ensures a CharacterController is attached to the GameObject
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    private Animator anim; // Reference to the Animator component for controlling animations
    private Camera cam; // Reference to the main camera
    private CharacterController controller; // Reference to the CharacterController component for movement

    private Vector3 desiredMoveDirection; // The direction the player wants to move based on input
    private Vector3 moveVector; // Vector used for vertical movement and gravity

    public Vector2 moveAxis; // Stores input from the player (e.g., WASD or joystick)
    private float verticalVel; // Vertical velocity for simulating gravity

    [Header("Settings")]
    [SerializeField] float movementSpeed; // Speed at which the player moves
    [SerializeField] float rotationSpeed = 0.1f; // Speed of player rotation
    [SerializeField] float fallSpeed = .2f; // Modifier for how fast the player falls
    public float acceleration = 1; // Modifier for movement speed scaling

    [Header("Booleans")]
    [SerializeField] bool blockRotationPlayer; // Determines if player rotation is locked (e.g., for strafing)
    private bool isGrounded; // Tracks if the player is on the ground

    void Start()
    {
        // Initialize references to components
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
    }

    void Update()
    {
        // Handle input and movement each frame
        InputMagnitude();

        // Check if the player is grounded
        isGrounded = controller.isGrounded;

        // Apply gravity when the player is not grounded
        if (isGrounded)
            verticalVel -= 0; // Reset vertical velocity
        else
            verticalVel -= 1; // Apply gravity over time

        // Apply vertical movement (e.g., falling)
        moveVector = new Vector3(0, verticalVel * fallSpeed * Time.deltaTime, 0);
        controller.Move(moveVector);
    }

    void PlayerMoveAndRotation()
    {
        // Get forward and right directions based on the camera's orientation
        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        // Ignore vertical components (y-axis)
        forward.y = 0f;
        right.y = 0f;

        // Normalize the vectors to maintain consistent magnitude
        forward.Normalize();
        right.Normalize();

        // Calculate the desired movement direction based on player input
        desiredMoveDirection = forward * moveAxis.y + right * moveAxis.x;

        if (blockRotationPlayer == false)
        {
            // Rotate the player towards the desired direction and move
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), rotationSpeed * acceleration);
            controller.Move(desiredMoveDirection * Time.deltaTime * (movementSpeed * acceleration));
        }
        else
        {
            // Handle strafing movement (no rotation)
            controller.Move((transform.forward * moveAxis.y + transform.right * moveAxis.y) * Time.deltaTime * (movementSpeed * acceleration));
        }
    }

    public void LookAt(Vector3 pos)
    {
        // Rotate the player to face a specific position
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), rotationSpeed);
    }

    public void RotateToCamera(Transform t)
    {
        // Rotate the player to align with the camera's forward direction
        var forward = cam.transform.forward;

        desiredMoveDirection = forward; // Set the desired move direction to the camera's forward vector
        Quaternion lookAtRotation = Quaternion.LookRotation(desiredMoveDirection);
        Quaternion lookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // Smoothly rotate the player to face the camera's forward direction
        t.rotation = Quaternion.Slerp(transform.rotation, lookAtRotationOnly_Y, rotationSpeed);
    }

    void InputMagnitude()
    {
        // Calculate the magnitude of player input (movement intensity)
        float inputMagnitude = new Vector2(moveAxis.x, moveAxis.y).sqrMagnitude;

        // Move the player if input magnitude is above a small threshold
        if (inputMagnitude > 0.1f)
        {
            anim.SetFloat("InputMagnitude", inputMagnitude * acceleration, .1f, Time.deltaTime); // Update the animation parameter
            PlayerMoveAndRotation(); // Perform movement and rotation
        }
        else
        {
            // Update the animation parameter with zero input
            anim.SetFloat("InputMagnitude", inputMagnitude * acceleration, .1f, Time.deltaTime);
        }
    }

    #region Input

    public void OnMove(InputValue value)
    {
        // Capture input from the player (e.g., WASD or joystick)
        moveAxis.x = value.Get<Vector2>().x;
        moveAxis.y = value.Get<Vector2>().y;
    }

    #endregion

    private void OnDisable()
    {
        // Reset animation parameter when the script is disabled
        anim.SetFloat("InputMagnitude", 0);
    }
}

