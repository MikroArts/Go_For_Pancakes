using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = .15f;
    Animator anim;
    public Vector3 offset;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
    void FixedUpdate()
    {
        Vector2 smoothPos = Vector2.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
        transform.position = smoothPos;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            offset = new Vector3(0, -3f, 0);
        }
        else//if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            offset = new Vector3(0, 1.5f, 0);
        }
    }
    public void ShakeCam()
    {
        anim.SetTrigger("isHit");
    }
}
