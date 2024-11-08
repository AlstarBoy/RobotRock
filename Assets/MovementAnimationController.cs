using Unity.Burst.Intrinsics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementAnimationController : MonoBehaviour
{
    public Animator animator;
    private TwinStickMovement tsm;
    public float yRotation;


    private void Awake()
    {
        tsm = GetComponent<TwinStickMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        //UnityEngine.Debug.Log("X == " + tsm.movement.x);
        //UnityEngine.Debug.Log("Y == " + tsm.movement.y);
        HandleMovementDirection();
    }

    void HandleMovementDirection()
    {
        yRotation = transform.rotation.eulerAngles.y;
        if (yRotation > 45 && yRotation < 135) // Facing Right
        {
            UnityEngine.Debug.Log("Right");
            animator.SetFloat("Horizontal", tsm.movement.y);
            animator.SetFloat("Vertical", tsm.movement.x);
        }
        else if (yRotation > 135 && yRotation < 225) // Facing Down
        {
            UnityEngine.Debug.Log("Down");
            animator.SetFloat("Horizontal", tsm.movement.x * -1);
            animator.SetFloat("Vertical", tsm.movement.y * -1);
        }
        else if (yRotation > 225 && yRotation < 315) // Facing Left
        {
            UnityEngine.Debug.Log("Left");
            animator.SetFloat("Horizontal", tsm.movement.y * -1);
            animator.SetFloat("Vertical", tsm.movement.x * -1);
        }
        else if (yRotation > 315 || yRotation > 0 || yRotation < 0) // Facing U
        {
            UnityEngine.Debug.Log("Up");
            animator.SetFloat("Horizontal", tsm.movement.x);
            animator.SetFloat("Vertical", tsm.movement.y);
        }


    }
}
