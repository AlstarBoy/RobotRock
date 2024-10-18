using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform player; // The player object to follow
    public Vector3 offset;   // Offset distance between the player and the camera
    public float smoothSpeed = 0.125f; // Smoothing speed factor

    private Vector3 velocity = Vector3.zero; // Reference velocity for SmoothDamp

    void LateUpdate()
    {
        if (player != null)
        {
            // Define the target position based on player's position and the offset
            Vector3 targetPosition = player.position + offset;

            // Smoothly move the camera towards the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
        }
    }
}

