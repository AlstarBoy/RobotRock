using UnityEngine;

public class RotationControls : MonoBehaviour
{
    public Transform pivotPoint;      // The point around which the camera rotates
    public float rotationAngle = 90f; // Degrees to rotate
    public float rotationSpeed = 2f;  // Speed of rotation

    private Quaternion targetRotation;
    private bool isRotating = false;

    void Start()
    {
        if (pivotPoint == null)
            pivotPoint = this.transform;

        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartRotation(-rotationAngle);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                StartRotation(rotationAngle);
            }
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }

    void StartRotation(float angle)
    {
        isRotating = true;
        targetRotation = Quaternion.AngleAxis(angle, Vector3.up) * transform.rotation;
    }
}
