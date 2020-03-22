using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = .15f;
    void LateUpdate()
    {
        Vector2 smoothPos = Vector2.Lerp(transform.position, target.position, smoothSpeed * Time.deltaTime);
        transform.position = smoothPos;
    }
}
