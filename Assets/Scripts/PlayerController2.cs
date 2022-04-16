using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float strafeMultiplier = 1.2f;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;
    [SerializeField] LayerMask groundLayer;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] float cameraTiltSmoothing = 0.01f; // should be around 1/100 of tilt modifier for best smoothing
    [SerializeField][Range(0.0f, 10f)] float cameraTiltMultiplier = 1.0f; // more than 1.0f gets rediculous fast

    [SerializeField] bool lockCursor = true;

    [SerializeField] Transform playerPosFollower;

    [SerializeField] Transform worldUp;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    float jumpForce;
    [HideInInspector]
    public Vector3 velocity;

    Vector2 targetDir = Vector2.zero;
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 targetMouseDelta = Vector2.zero;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    bool canDash = false;
    float _dashTime;

    bool isGrounded = false;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        canDash = true;
        _dashTime = dashTime;
    }

    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
        
        isGrounded = Physics.CheckSphere(transform.position, 0.07f, groundLayer);
    }

    void FixedUpdate()
    {
        
    }

    void UpdateMouseLook()
    {
        targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        float cameraTilt = cameraTiltMultiplier * -1 * targetDir.x;

        playerCamera.localEulerAngles = Vector3.right * cameraPitch + Vector3.forward * Mathf.LerpAngle(playerCamera.localEulerAngles.z, cameraTilt, cameraTiltSmoothing);
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        targetDir.x *= strafeMultiplier;

        if(isGrounded)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                jumpForce = Mathf.Sqrt(jumpHeight * -2 * gravity);
                rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
            }
            if(!Input.GetKeyDown(KeyCode.Space))
            {
                jumpForce = 0.0f;
            }

            velocityY = Mathf.Clamp(velocityY, 0.0f, 100.0f);
        }

        velocityY += gravity * Time.deltaTime;

        rb.AddForce((transform.forward * targetDir.y + transform.right * targetDir.x) * walkSpeed, ForceMode.Force);
        rb.velocity += new Vector3(0, velocityY * 0.2f, 0);
    }
}
