using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 3;
    public int lives = 3;
    public int points;

    Rigidbody2D rb;
    Animator anim;
    Transform cam;
    public float camSmoothSpeed = 2.15f;

    float moveInput;
    public float jumpForce;
    public float speed;

    
    bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask ground;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera").transform;        
    }
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
        moveInput = Input.GetAxis("Horizontal");        

        if (moveInput == 0)
        {
            anim.SetBool("isRide", false);
        }
        else
        {
            rb.velocity = new Vector2(moveInput * speed * Time.deltaTime, rb.velocity.y);
            anim.SetBool("isRide", true);
            if (moveInput < 0)
                transform.eulerAngles = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            else
                transform.eulerAngles = new Vector3(transform.rotation.x, 0, transform.rotation.z);            
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            anim.SetTrigger("isJump");
            rb.velocity = Vector2.up * jumpForce;
        }

        Vector2 smoothPos = Vector2.Lerp(cam.position, transform.position, camSmoothSpeed * Time.deltaTime);
        cam.position = smoothPos;
    }
}
