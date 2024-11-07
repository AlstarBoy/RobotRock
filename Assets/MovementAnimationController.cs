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
    private float yRotation;

    private void Awake()
    {
        tsm = GetComponent<TwinStickMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Horizontal", tsm.movement.x);
        animator.SetFloat("Vertical", tsm.movement.y);
        UnityEngine.Debug.Log("X == " + tsm.movement.x);
        UnityEngine.Debug.Log("Y == " + tsm.movement.y);
        HandleMovementDirection();
    }

    void HandleMovementDirection()
    {
        yRotation = this.gameObject.transform.rotation.y;
        if (yRotation > 45)
        {

        }
        if (yRotation > 45)
        {

        }
        if (yRotation > 45)
        {

        }
        if (yRotation > 45)
        {

        }

    }
}
