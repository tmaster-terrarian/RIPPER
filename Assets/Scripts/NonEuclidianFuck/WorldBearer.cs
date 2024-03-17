using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBearer : MonoBehaviour
{
    public Transform childHolder = null;
    public Transform target = null;
    public Vector3 offset = Vector3.zero;

    Vector3 startPos = Vector3.zero;
    Vector3 _offset = Vector3.zero;

    Vector3 currentRot;
    Vector3 targetRot;
    Vector3 currentRotVelocity = Vector3.zero;
    float rotSmoothTime = 0.15f;

    float rotTime = 0.5f;
    bool canRotate = true;

    void Awake()
    {
        _offset = offset.x * target.right + offset.y * target.up + offset.z * target.forward;
        startPos.x = target.position.x;
        startPos.y = target.position.y;
        startPos.z = target.position.z;
    }

    void LateUpdate()
    {
        _offset = offset.x * target.right + offset.y * target.up + offset.z * target.forward;

        currentRot = Vector3.SmoothDamp(currentRot, targetRot, ref currentRotVelocity, rotSmoothTime);

        if(Input.GetKeyDown(KeyCode.Alpha3) && canRotate)
        {
            canRotate = false;
            Rotate(new Vector3(90, 0, 0));
        }
    }

    void Rotate(Vector3 rotation)
    {
        targetRot = rotation;
        StartCoroutine(RotateSmooth());
        StartCoroutine(RotateSmoothCooldown());
    }

    IEnumerator RotateSmooth()
    {
        float startTime = Time.time;

        while(Time.time < startTime + rotTime)
        {
            transform.RotateAround(target.position + _offset, Vector3.right,   targetRot.x / (rotTime * 60));
            transform.RotateAround(target.position + _offset, Vector3.up,      targetRot.y / (rotTime * 60));
            transform.RotateAround(target.position + _offset, Vector3.forward, targetRot.z / (rotTime * 60));

            yield return null;
        }
    }
    IEnumerator RotateSmoothCooldown()
    {
        yield return new WaitForSeconds(rotTime + 0.1f);
        canRotate = true;
    }
}
