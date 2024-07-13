using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUp : MonoBehaviour
{
    public float worldGravity = -9.81f;

    void Start()
    {
        
    }

    void Update()
    {
        Physics.gravity = transform.up * worldGravity;
    }
}
