using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation3DObject : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}