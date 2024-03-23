using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using bsc;

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
    float velocityX = 0.0f;
    float velocityY = 0.0f;
    float jumpForce;
    [HideInInspector] public Vector3 velocity { get; protected set; }
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

    Slider[] crosshairSegments;
    Ticker recoil = new Ticker();
    float maxRecoil = 4;

    Ticker firedelay = new Ticker();
    float fireinterval = 10;

    public float atkSpd = 1;

    GameObject viewModel = null;
    Animator viewModelAnimator = null;
    GameObject viewModel2 = null;
    Animator viewModelAnimator2 = null;
    float alternator = 1;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        viewModel = GameObject.FindGameObjectsWithTag("VIEWMODEL")[0];
        viewModelAnimator = viewModel.GetComponent<Animator>();
        viewModel2 = GameObject.FindGameObjectsWithTag("VIEWMODEL")[1];
        viewModelAnimator2 = viewModel2.GetComponent<Animator>();

        recoil.Reconfigure(maxRecoil, () => {return;});
        recoil.rate = 0.2f;
        firedelay.Reconfigure(fireinterval / atkSpd, () => {return;});

        crosshairSegments = new Slider[4] {
            GameObject.FindGameObjectWithTag("CrosshairW").GetComponent<Slider>(),
            GameObject.FindGameObjectWithTag("CrosshairE").GetComponent<Slider>(),
            GameObject.FindGameObjectWithTag("CrosshairN").GetComponent<Slider>(),
            GameObject.FindGameObjectWithTag("CrosshairS").GetComponent<Slider>()
        };

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
        isGrounded = Physics.CheckSphere(transform.position, 0.07f, groundLayer) || controller.isGrounded;

        UpdateMouseLook();
        UpdateAttacks();
        UpdateMovement();
        UpdateDash();
        CollisionDetectionFix();

        if(transform.position.y <= -50.0f)
        {
            transform.position = Vector3.up;
            velocity = Vector3.zero;
            jumpForce = 0f;
            velocityX = 0f;
            velocityY = 0f;
        }

        playerPosFollower.position = transform.position;

        UpdateCrosshair();
    }

    void FixedUpdate()
    {
        UpdateTickers();
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

    void UpdateAttacks()
    {
        if(Input.GetMouseButton(0) && firedelay.isDone)
        {
            firedelay.Reset();
            recoil.value = Mathf.Min(recoil.value + 2.5f, maxRecoil);

            viewModelAnimator.SetFloat("atkSpd", atkSpd);
            viewModelAnimator2.SetFloat("atkSpd", atkSpd);

            if(alternator == 1)
            {
                viewModelAnimator.SetBool("firing", true);
                viewModelAnimator2.SetBool("firing", false);
            }
            else
            {
                viewModelAnimator.SetBool("firing", false);
                viewModelAnimator2.SetBool("firing", true);
            }

            alternator = -alternator;

            SFX.Play($"mando_M1_{UnityEngine.Random.Range(1, 12)}", 0.1f);
        }
        else
        {
            viewModelAnimator.SetBool("firing", false);
            viewModelAnimator2.SetBool("firing", false);
        }
    }

    void UpdateMovement()
    {
        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        targetDir.x *= strafeMultiplier;

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if(isGrounded)
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
        Vector3 _velX = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed;

        velocity = _velX + (worldUp.up * jumpForce) + (worldUp.up * velocityY);

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
            Vector3 vel = new Vector3(velocity.x, 0, velocity.z);
            controller.Move(vel * dashSpeed * Time.deltaTime);

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
        Vector3 p1 = transform.position + worldUp.up * 0.25f;
        //Top of controller
        Vector3 p2 = p1 + worldUp.up * controller.height;

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
        if (Physics.Raycast(transform.position + worldUp.up, -worldUp.up, out hit, 1, 1 << groundLayer))
        {
            controller.Move(Vector3.up * (1 - hit.distance));
        }

        RaycastHit jumpStopHit;

        if(Physics.Raycast(transform.position, worldUp.up, out jumpStopHit, controller.height + 0.06f, groundLayer))
        {
            jumpForce = 0;
            velocityY = Mathf.Max(velocityY, 0);
        }
    }

    void UpdateTickers()
    {
        recoil.Update();
        firedelay.Update();
    }

    void UpdateCrosshair()
    {
        foreach(Slider slider in crosshairSegments)
        {
            try
            {
                slider.value = recoil.value / maxRecoil;
            }
            catch(Exception e)
            {
                Debug.LogException(e, this.gameObject);
                return;
            }
        }
    }
}
