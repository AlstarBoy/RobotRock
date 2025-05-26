using UnityEngine;
using UnityEngine.InputSystem;


public class MouseMover : MonoBehaviour
{
    [SerializeField] private Camera _cam;  // Reference to the main camera
    [SerializeField] private float _yPosition = 0f; // Fixed Y position for movement

    // Highlight Block
    public GameObject hBlock;
    //[SerializeField] private bool blockUpdate;
    //[SerializeField] private Vector3 previousPosition = Vector3.zero;

    [SerializeField] private PlayerInput _playerInput;
    private InputAction _pointAction;

    private void Awake()
    {
        // Retrieve the 'Point' action from the PlayerInput's action map
        _pointAction = _playerInput.actions["Move"];
    }


    void Update()
    {
        MoveObjectToMouse();
        //hBlock.transform.position = new Vector3(1,1,1);
    }

    void MoveObjectToMouse()
    {
        // Create a ray from the mouse position
        Vector2 screenPosition = _pointAction.ReadValue<Vector2>();
        Ray ray = _cam.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, _yPosition, 0));

        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            // Get the point where the ray hits the plane
            Vector3 targetPosition = ray.GetPoint(rayDistance);

            // Move the GameObject to this position (keeping Y fixed)
            transform.position = new Vector3(targetPosition.x, _yPosition, targetPosition.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        print("trigger");
        if (other.CompareTag("tile"))
        {
            print("move");
            hBlock.transform.position = other.transform.position;
        }
    }

    void OnTriggerExit(Collider other)
    {

    }
}
