using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform target;
    public float smoothSpeed = 2f;
    Animator anim;
    public Vector3 offset;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void FixedUpdate()
    {
        Vector2 smoothPos = Vector2.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
        transform.position = smoothPos;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            offset = new Vector3(0, -3f, 0);
        }
        else
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                offset = new Vector3(1f, 1.5f, 0);
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                offset = new Vector3(-1f, 1.5f, 0);
            else
                offset = new Vector3(0, 1.5f, 0);
        }
    }
    public void ShakeCam()
    {
        anim.SetTrigger("isHit");
    }
}
