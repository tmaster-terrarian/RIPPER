using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPlayerController : MonoBehaviour
{
    SuperCharacterController characterController;
    [SerializeField] bool lockCursor = true;
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float strafeMultiplier = 1.2f;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] float cameraTiltSmoothing = 0.01f; // should be around 1/100 of tilt modifier for best smoothing
    [SerializeField][Range(0.0f, 10f)] float cameraTiltMultiplier = 1.0f; // more than 1.0f gets rediculous fast
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

    Vector3 lastPosition;

    void Start()
    {
        characterController = new SuperCharacterController();
        characterController.DisableClamping();

        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        lastPosition = transform.position;
    }

    void SuperUpdate()
    {
        UpdateMouseLook();
        UpdateMovement();
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

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        bool isGrounded = Physics.CheckSphere(transform.position, 0.07f, characterController.Walkable);

        if(isGrounded)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                jumpForce = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
            if(!Input.GetKeyDown(KeyCode.Space))
            {
                jumpForce = 0.0f;
            }

            velocityY = Mathf.Clamp(velocityY, 0.0f, 100.0f);
        }

        velocityY += gravity * Time.deltaTime;

        velocity = ((transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed)/* + (worldUp.up * jumpForce) + (worldUp.up * velocityY)*/;

        if(targetDir != Vector2.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, velocity + lastPosition, 5 * Time.deltaTime);
        }
        lastPosition = transform.position;
    }
}
