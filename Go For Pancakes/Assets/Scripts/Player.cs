using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 5;
    public int numOfHearts;
    public Image[] hearts;
    public Sprite heart;
    public int lives = 3;
    public int points;

    Rigidbody2D rb;
    Animator anim;

    float moveInput;
    public float jumpForce;
    public float speed;

    public GameObject[] particles;
    
    bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask ground;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();     
    }
    void Update()
    {
        numOfHearts = health;        
        for (int i = 0; i < hearts.Length; i++)
        {            
            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
        if (health <= 0)
        {
            print("Dead");
        }
        Jump();
    }
    void FixedUpdate()
    {        
        Move();
    }

    private void Move()
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
    }
    private void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
            anim.SetTrigger("isJump");
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Book"))
        {
            Instantiate(particles[0], col.transform.position, Quaternion.identity);
            Destroy(col.gameObject);            
        }
        if (col.CompareTag("Cop"))
        {
            health -= col.GetComponent<PoliceMan>().damage;
            Instantiate(particles[1], col.transform.position, Quaternion.identity);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraFollow>().ShakeCam();
        }
        if (col.CompareTag("Granny"))
        {
            health -= col.GetComponent<Granny>().damage;
            Instantiate(particles[2], col.transform.position, Quaternion.identity);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraFollow>().ShakeCam();
        }
    }
}
