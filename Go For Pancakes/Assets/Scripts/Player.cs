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
    public Text livesText;

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

    private bool isCollide;
    public float timer;
    

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

        if (lives >= 0)
        {
            if (health <= 0)
            {
                lives--;
                health += 7;
            }
            else
                livesText.text = "x" + lives;
        }
        else
        {
            livesText.text = "x0";
            health = 0;
            print("Game Over");
            return;
        }

        Jump();

        if (isCollide)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                timer = 0;
        }
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
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;

                if (health >= 0 && lives > -1)
                    health -= col.GetComponent<PoliceMan>().damage;
                else
                    return;
                Instantiate(particles[1], col.transform.position, Quaternion.identity);
                GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraFollow>().ShakeCam();
            }            
        }
        if (col.CompareTag("Granny"))
        {
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;

                if (health >= 0 && lives > -1)
                    health -= col.GetComponent<Granny>().damage;
                else
                    return;

                Instantiate(particles[2], col.transform.position, Quaternion.identity);
                GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraFollow>().ShakeCam();
            }
            
        }
    }
}
