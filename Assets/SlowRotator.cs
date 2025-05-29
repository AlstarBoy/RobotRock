using UnityEngine;

public class SlowRotator : MonoBehaviour
{
    // Rotation speed in degrees per second
    public Vector3 rotationSpeed = new Vector3(0f, 10f, 0f);

    void Update()
    {
        // Rotate the object by the given speed, scaled by deltaTime
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}