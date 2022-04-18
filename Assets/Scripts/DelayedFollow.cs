using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedFollow : MonoBehaviour
{
    Vector3 startPos;
    [SerializeField] float delayIntensityMouse = 1.3f;
    [SerializeField] float smoothAmountMouse = 1;
    [SerializeField] Vector2 maxDistanceMouse;
    [SerializeField] float delayIntensityMovement = 1.3f;
    [SerializeField] float smoothAmountMovement = 1;
    [SerializeField] Vector2 maxDistanceMovement;

    void OnEnable()
    {
        startPos = transform.localPosition;
    }

    void FixedUpdate()
    {
        FollowMouse();
        FollowPosition();
    }

    void FollowMouse()
    {
        Vector2 mouseDelta = new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * delayIntensityMouse;
        mouseDelta.x = Mathf.Clamp(mouseDelta.x, -maxDistanceMouse.x, maxDistanceMouse.x);
        mouseDelta.y = Mathf.Clamp(mouseDelta.y, -maxDistanceMouse.y, maxDistanceMouse.y);

        Vector3 targetPos = new Vector3(mouseDelta.x, mouseDelta.y, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos + startPos, Time.fixedDeltaTime * smoothAmountMouse);
    }

    void FollowPosition()
    {
        Vector2 moveDelta = new Vector2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * delayIntensityMovement;
        //moveDelta = Utils.ClampVector2(moveDelta, -maxDistanceMovement, maxDistanceMovement);
        moveDelta.x = Mathf.Clamp(moveDelta.x, -maxDistanceMovement.x, maxDistanceMovement.x);
        moveDelta.y = Mathf.Clamp(moveDelta.y, -maxDistanceMovement.y, maxDistanceMovement.y);

        Vector3 targetPos = new Vector3(moveDelta.x, 0, moveDelta.y);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos + startPos, Time.fixedDeltaTime * smoothAmountMovement);
    }
}
