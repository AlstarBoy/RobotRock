using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class placingObject : MonoBehaviour
{
    public GameObject mousePos;
    [Header("Placed Variables")]
    private bool idleRot = false;
    public bool isPlaced = false;
    public GameController gameC;
    private Vector3 placePosition;
    [Header("Falling Variables")]
    [SerializeField] private float baseMoveSpeed = 3f; // Base speed for movement
    [SerializeField] private float accelerationFactor = 6f; // Multiplier for acceleration
    [SerializeField] private float fallSpeed = 0.1f; // Speed of falling (Y axis)
    [SerializeField] private float groundY = 0f; // Y position to stop falling
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation adjustment
    [SerializeField] private float idleRotationSpeed = 5f; // Speed of rotation adjustment
    public bool fastPlace = false;
    public GameObject artWork;

    [Header("Score")]
    private ScoreSystem scoreSystem;
    public int scoreWhenPlaced = 50;


    private void Awake()
    {
        if (scoreSystem == null)
        {
            scoreSystem = GameObject.Find("=== Score System").GetComponent<ScoreSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaced)
        {
            MoveTowardsTarget();
            RotateTowardsTarget();
            if (transform.position.y == 0)
            {
                placeOnGrid();
            }
        }
        if (isPlaced && idleRot == false)
        {
            transform.rotation = Quaternion.Euler(-90f, 0, 0f);
            idleRot = true;
        }
        if (idleRot)
        {
            artWork.transform.Rotate(0, idleRotationSpeed * Time.deltaTime, 0);
        }
        if (fastPlace)
        {
            // Apply falling effect - moving down smoothly
            float newY = Mathf.Max(transform.position.y - Mathf.Abs(fallSpeed) * 50f * Time.deltaTime, groundY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    public void placeObjectSmoothly()
    {
        fastPlace = true;
    }

    public void placeOnGrid()
    { 
        if (!isPlaced)
        {
            placePosition = mousePos.transform.position;
            transform.position = new Vector3(placePosition.x, 0, placePosition.z);
            isPlaced = true;
            gameC.objectPlaced = true;
            scoreSystem.IncreaseScore(scoreWhenPlaced);
        }
    }

    void MoveTowardsTarget()
    {
        // Get current position
        Vector3 currentPosition = transform.position;
        // Get target XZ position while keeping the current Y position
        Vector3 targetXZ = new Vector3(mousePos.transform.position.x, transform.position.y, mousePos.transform.position.z);

        // Calculate distance to target in XZ plane
        float distance = Vector3.Distance(new Vector3(currentPosition.x, 0, currentPosition.z),
                                          new Vector3(mousePos.transform.position.x, 0, mousePos.transform.position.z));

        // Speed increases the further the object is from the target
        float dynamicSpeed = baseMoveSpeed + (distance * accelerationFactor);

        // Move towards the target at a speed proportional to the distance
        transform.position = Vector3.MoveTowards(currentPosition, targetXZ, dynamicSpeed * Time.deltaTime);

        // Apply falling effect - moving down smoothly
        float newY = Mathf.Max(currentPosition.y - Mathf.Abs(fallSpeed) * 0.5f * Time.deltaTime, groundY);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

    }
    void RotateTowardsTarget()
    {
        // Get direction to target
        Vector3 direction = (mousePos.transform.position - transform.position).normalized;

        // Avoid unnecessary rotation when stationary
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
