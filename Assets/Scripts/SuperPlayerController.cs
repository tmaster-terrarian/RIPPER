using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPlayerController : SuperStateMachine
{
    SuperCharacterController controller;
    Vector2 targetDir = Vector2.zero;

    private enum PlayerState { Idle, Walk, Jump, Fall }

    private bool AcquiringGround => controller.currentGround.IsGrounded(false, 0.01f);
    private bool MaintainingGround => controller.currentGround.IsGrounded(true, 0.5f);

    [SerializeField] Transform orientation;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] float jumpHeight = 2.0f;

    public float JumpForce => Mathf.Sqrt(jumpHeight * -2 * gravity);

    [HideInInspector] public Vector3 Velocity = Vector3.zero;
    public bool IsGrounded { get; private set; }

    public Vector2 InputDir => targetDir.normalized;

    void Start()
    {
        controller = GetComponent<SuperCharacterController>();
        // controller.maxClampingDistance = Mathf.Abs(JumpForce) * 1/60f;

        currentState = PlayerState.Idle;
    }

    void Update()
    {
        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        // Vector2 _target = new(targetDir.x * strafeMultiplier, targetDir.y);

        // Velocity = Vector2.SmoothDamp(Velocity, _target, ref currentDirVelocity, moveSmoothTime);
    }

    protected override void EarlyGlobalSuperUpdate()
    {
        // Put any code in here you want to run BEFORE the state's update function.
        // This is run regardless of what state you're in
    }

    protected override void LateGlobalSuperUpdate()
    {
        // Put any code in here you want to run AFTER the state's update function.
        // This is run regardless of what state you're in

        // Move the player by our velocity every frame
        transform.position += Velocity * controller.deltaTime;

        if(transform.position.y <= -50.0f)
        {
            transform.position = Vector3.zero;
            Velocity = Vector3.zero;
            currentState = PlayerState.Idle;
            controller.EnableSlopeLimit();
            controller.EnableClamping();
        }
    }

    private Vector3 LocalMovement()
    {
        Vector3 local = Vector3.zero;

        if(targetDir != Vector2.zero)
        {
            local += orientation.forward * targetDir.y + orientation.right * targetDir.x;
        }

        return local.normalized;
    }

    #region Idle State

    void Idle_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();
    }

    void Idle_SuperUpdate()
    {
        // Run every frame we are in the idle state

        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentState = PlayerState.Jump;
            return;
        }

        if(!MaintainingGround)
        {
            currentState = PlayerState.Fall;
            return;
        }

        if(targetDir != Vector2.zero)
        {
            currentState = PlayerState.Walk;
            return;
        }

        // Apply friction to slow us to a halt
        Velocity = Vector3.MoveTowards(Velocity, Vector3.zero, 50 * controller.deltaTime);
    }

    void Idle_ExitState()
    {
        // Run once when we exit the idle state
    }

    #endregion

    #region Walk State

    void Walk_SuperUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentState = PlayerState.Jump;
            return;
        }

        if(!MaintainingGround)
        {
            currentState = PlayerState.Fall;
            return;
        }

        if(targetDir != Vector2.zero)
        {
            Velocity = Vector3.MoveTowards(Velocity, LocalMovement() * walkSpeed, 40 * controller.deltaTime);
        }
        else
        {
            currentState = PlayerState.Idle;
            return;
        }
    }

    #endregion

    #region Jump State

    void Jump_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        Velocity += controller.up * JumpForce;
    }

    void Jump_SuperUpdate()
    {
        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, Velocity);
        Vector3 verticalMoveDirection = Velocity - planarMoveDirection;

        if(Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround)
        {
            Velocity = planarMoveDirection;
            currentState = PlayerState.Idle;
            return;            
        }

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * walkSpeed, 40 * controller.deltaTime);
        verticalMoveDirection += controller.up * gravity * controller.deltaTime;

        Velocity = planarMoveDirection + verticalMoveDirection;
    }

    #endregion

    #region Fall State

    void Fall_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        // moveDirection = trueVelocity;
    }

    void Fall_SuperUpdate()
    {
        if(AcquiringGround)
        {
            Velocity = Math3d.ProjectVectorOnPlane(controller.up, Velocity);
            currentState = PlayerState.Idle;
            return;
        }

        Velocity += controller.up * gravity * controller.deltaTime;
    }

    #endregion
}
