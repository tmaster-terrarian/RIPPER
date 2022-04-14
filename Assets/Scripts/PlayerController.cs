using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    //[SerializeField] float dashZoomMultiplier;
    [SerializeField] LayerMask groundLayer;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] float cameraTiltSmoothing = 0.01f; // should be around 1/100 of tilt modifier for best smoothing
    [SerializeField][Range(0.0f, 10f)] float cameraTiltMultiplier = 1.0f; // more than 1.0f gets rediculous fast

    [SerializeField] bool lockCursor = true;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    float jumpForce;
    Vector3 velocity;
    CharacterController controller = null;

    Vector2 targetDir = Vector2.zero;
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 targetMouseDelta = Vector2.zero;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    bool canDash = false;
    float _dashTime;

    bool isGrounded = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
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
        UpdateDash();

        isGrounded = Physics.CheckBox(transform.position, new Vector3(0.45f, 0.05f, 0.45f), Quaternion.identity, groundLayer);
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

        if(controller.isGrounded)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                jumpForce = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
            else
            {
                jumpForce = 0.0f;
            }

            velocityY = Mathf.Clamp(velocityY, 0.0f, 100.0f);
        }

        velocityY += gravity * Time.deltaTime;

        velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * jumpForce + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateDash()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash == true)
        {
            canDash = false;
            StartCoroutine(Dash());
            StartCoroutine(DashCooldown());
        }
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            controller.Move(velocity * dashSpeed * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
