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

    [SerializeField] Transform playerPosFollower;

    [SerializeField] Transform worldUp;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    float jumpForce;
    [HideInInspector]
    public Vector3 velocity;
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

    Rigidbody rb;

    float distance;

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
        CollisionDetectionFix();

        isGrounded = Physics.CheckSphere(transform.position, 0.07f, groundLayer);

        playerPosFollower.position = transform.position;
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

        if(isGrounded || controller.isGrounded)
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

        velocity = ((transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed) + (worldUp.up * jumpForce) + (worldUp.up * velocityY);

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

    void CollisionDetectionFix()
    {
        RaycastHit hit;

        //Bottom of controller. Slightly above ground so it doesn't bump into slanted platforms. (Adjust to your needs)
        Vector3 p1 = transform.position + Vector3.up * 0.25f;
        //Top of controller
        Vector3 p2 = p1 + Vector3.up * controller.height;

        //Check around the character in a 360, 10 times (increase if more accuracy is needed)
        for(int i=0; i<360; i+= 36)
        {
            //Check if anything with the platform layer touches this object
            if (Physics.CapsuleCast(p1, p2, 0, new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i)), out hit, distance, 1 << groundLayer))
            {
                //If the object is touched by a platform, move the object away from it
                controller.Move(hit.normal*(distance - hit.distance));
            }
        }

        //[Optional] Check the players feet and push them up if something clips through their feet.
        //(Useful for vertical moving platforms)
        if (Physics.Raycast(transform.position + Vector3.up, - Vector3.up, out hit, 1, 1 << groundLayer))
        {
            controller.Move(Vector3.up * (1 - hit.distance));
        }
    }
}
