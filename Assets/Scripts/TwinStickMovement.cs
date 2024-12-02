using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class TwinStickMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float controllerDeadzone = 0.1f;
    [SerializeField] private float gamepadRotateSmoothing = 1000f;

    [SerializeField] private bool isGamepad;
    public bool combat;

    private CharacterController controller;

    public Vector2 movement;
    private Vector2 aim;

    private Vector3 playerVelocity;
    public Vector3 playerDirection;

    private PlayerControls playerControls;
    private PlayerInput playerInput;

    private Vector3 sphereTest;

    public float rotationSpeed = 10f;

    // Character Model Rotation
    public bool canRotate = false;
    public GameObject charModel;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    void Update()
    {
        HandleInput();
        if(!combat)
        {
            HandleMovement();
        }
        //HandleMouseRotation();
    }

    void HandleInput()
    {
            movement = playerControls.Controls.Move.ReadValue<Vector2>(); 
            aim = playerControls.Controls.Look.ReadValue<Vector2>();
    }
    public void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Rotate the model towards movement direction if there is movement
        if (move != Vector3.zero)
        {
            // Calculate target rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move);

            // Smoothly rotate towards the target rotation (optional: smoothing for better effect)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void CombatAim(GameObject gameObject)
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        // Rotate the model towards movement direction if there is movement
        if (move != Vector3.zero)
        {
            // Calculate target rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move);

            gameObject.transform.rotation = targetRotation;
            // Smoothly rotate towards the target rotation (optional: smoothing for better effect)
            //transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void onDeviceChange(PlayerInput pi)
    {
        isGamepad = pi.currentControlScheme.Equals("Gamepad") ? true : false;

    }

    /*
    void HandleMouseRotation()
    {
            if (isGamepad)
            {
                //UnityEngine.Debug.Log("GamePad");
                //Rotate our player
                if (Math.Abs(aim.x) > controllerDeadzone || Mathf.Abs(aim.y) > controllerDeadzone)
                {
                    playerDirection = Vector3.right * aim.x + Vector3.forward * aim.y;
                    if (playerDirection.sqrMagnitude > 0.0f)
                    {
                        Quaternion newrotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, newrotation, gamepadRotateSmoothing * Time.deltaTime);
                    }

                }
            }
            else
            {
                //UnityEngine.Debug.Log("Mouse");
                Ray ray = Camera.main.ScreenPointToRay(aim);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                float rayDistance;

                //UnityEngine.Debug.DrawRay(transform.position, forward, Color.green);


                if (groundPlane.Raycast(ray, out rayDistance))
                {
                    UnityEngine.Debug.Log("CAST HIT");
                    Vector3 point = ray.GetPoint(rayDistance);
                    LookAt(point);
                    sphereTest = point;
                }
            }
    }

    private void characterModelRotation()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        // Rotate the model towards movement direction if there is movement
        if (move != Vector3.zero)
        {
            // Calculate target rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move);

            // Smoothly rotate towards the target rotation (optional: smoothing for better effect)
            charModel.transform.rotation = Quaternion.Slerp(charModel.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }


    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(sphereTest, 1);
    }

    */


}