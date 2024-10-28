using System.Diagnostics;
using UnityEngine;

public class ArtController : MonoBehaviour
{
    public Vector3 WorldPoint;
    public Vector3 Difference;
    public float RotationY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        WorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Difference = WorldPoint - transform.position;
        Difference.Normalize();

        RotationY = Mathf.Atan2(Difference.x, Difference.z) * Mathf.Rad2Deg;
        transform.LookAt(Difference, Vector3.up);

    }
}
