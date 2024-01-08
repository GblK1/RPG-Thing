using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerInputs inputs;
    [SerializeField] private bool drawGizmos;
    [Header("BasicMovement")]
    [HideInInspector] public bool canMove;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float airborneAcceleration;
    [SerializeField] private float jumpPower;
    [SerializeField] private float gravityScale;
    private float gravity;
    private CharacterController cc;
    private Vector3 movementVector;
    [Space]
    [Header("CameraAndPlayerDirections")]
    private Transform orientation;
    private CameraController camController;
    [Space]
    [Header("GroundDetection")]
    [SerializeField] private float toGroundDistance;
    [SerializeField] private float chechSphereRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float GroundAngle;
    [SerializeField] private bool isGrounded;
    private RaycastHit groundHit;
    [Space]
    [Header("BodyAnims")]
    [SerializeField] private Transform body;
    [SerializeField] private Animator bodyAnims;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        camController = GetComponent<CameraController>();

        orientation = camController.orientation;

        inputs = new PlayerInputs();
        inputs.Enable();

        inputs.Main.Jump.performed += x => Jump();

        canMove = true;

        bodyAnims = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position - transform.up * toGroundDistance, chechSphereRadius, groundLayer);

        bodyAnims.SetBool("IsAirborne", !isGrounded);
    }

    private void Update()
    {
        Movement(inputs.Main.Movement.ReadValue<Vector2>());
    }

    private void Movement(Vector2 mv)
    {
        Vector3 movementDirection = orientation.forward * mv.y + orientation.right * mv.x;
        Physics.Raycast(transform.position, -transform.up, out groundHit, 4);

        if (canMove)
        {
            if (isGrounded)
            {
                movementVector = Vector3.MoveTowards(movementVector, movementDirection * speed, acceleration * Time.deltaTime);

                movementVector = Vector3.ProjectOnPlane(movementVector, groundHit.normal);

                if (mv != Vector2.zero)
                {
                    bodyAnims.SetBool("IsRunning", true);
                }
                else
                {
                    bodyAnims.SetBool("IsRunning", false);
                }
            }
            else
            {
                bodyAnims.SetBool("IsRunning", false);

                gravity -= Time.deltaTime * gravityScale;

                movementVector = Vector3.MoveTowards(movementVector, movementDirection * speed, airborneAcceleration * Time.deltaTime);
            }
        }

        body.LookAt(transform.position + movementVector);

        cc.Move(movementVector * Time.deltaTime);
        cc.Move(transform.up * gravity * Time.deltaTime);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            gravity = jumpPower;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isGrounded && Vector3.Angle(hit.normal, Vector3.up) < GroundAngle)
        {
            gravity = 0;
        }
        else
        {
            movementVector = Vector3.ProjectOnPlane(movementVector, hit.normal);
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(transform.position - transform.up * toGroundDistance, chechSphereRadius);
        }
    }
}
