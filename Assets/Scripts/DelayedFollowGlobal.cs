using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedFollowGlobal : MonoBehaviour
{
    Vector3 startPos;
    Vector3 lastPos;
    Vector3 delta;
    [SerializeField] float delayIntensity = 1.3f;
    [SerializeField] float smoothAmount = 1;
    [SerializeField] Vector3 maxDistance;
    [SerializeField] Transform followTarget;

    float maxDistanceX;
    float maxDistanceY;
    float maxDistanceZ;

    void OnEnable()
    {
        startPos = transform.position;
        lastPos = followTarget.position;

        maxDistanceX = maxDistance.x;
        maxDistanceY = maxDistance.y;
        maxDistanceZ = maxDistance.z;
    }

    void Update()
    {
        delta = -(followTarget.position - lastPos) * delayIntensity;
        delta.x = Mathf.Clamp(delta.x, -maxDistanceX, maxDistanceX);
        delta.y = Mathf.Clamp(delta.y, -maxDistanceY, maxDistanceY);
        delta.z = Mathf.Clamp(delta.z, -maxDistanceZ, maxDistanceZ);
        Vector3 targetPos = delta;

        transform.localPosition = Vector3.Lerp(transform.localPosition, GetComponentInParent<PlayerController>().velocity.normalized * delayIntensity + startPos, Time.fixedDeltaTime * smoothAmount);

        lastPos = followTarget.position;
    }
}
