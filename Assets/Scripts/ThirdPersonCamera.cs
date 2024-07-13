using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public SuperPlayerController player;

    [Header("Collision Logic")]
    public bool collidesWithWorld = true;
    public LayerMask layerMask;
    public float colliderRadius = 0.12f;

    [Header("Input")]
    public float rotationSpeed;
    public bool lockCursor = true;

    float currentDistance = 8;

    public float Pitch => transform.localEulerAngles.x;
    public float Yaw => transform.localEulerAngles.y;

    private void Start()
    {
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void FixedUpdate()
    {
        // rotate orientation
        Vector3 viewDir = player.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // rotate player object
        Vector3 inputDir = orientation.forward * player.InputDir.y + orientation.right * player.InputDir.x;

        if (inputDir != Vector3.zero)
            player.transform.forward = Vector3.Slerp(player.transform.forward, inputDir.normalized, Time.fixedDeltaTime * rotationSpeed);
    }

    void LateUpdate()
    {
        Vector3 targetPos = player.transform.position + Vector3.up * 2.5f;
        Vector3 direction = (transform.position - targetPos).normalized;

        currentDistance = (transform.position - targetPos).magnitude;

        if(Physics.SphereCast(player.transform.position + Vector3.up, colliderRadius, Vector3.up, out RaycastHit ceilingHit, 1.2f, layerMask, QueryTriggerInteraction.Ignore))
        {
            targetPos = player.transform.position + Vector3.up + Vector3.up * ceilingHit.distance * 0.9f;
            transform.position = targetPos + direction * currentDistance;
        }

        if(Physics.SphereCast(targetPos, colliderRadius, direction, out RaycastHit hit, currentDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = targetPos + direction * hit.distance;
            return;
        }
    }
}
