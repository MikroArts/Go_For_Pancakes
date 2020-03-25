using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    GameController gc;

    Rigidbody2D rb;
    Animator anim;

    [Header("Hearts Data")]
    public int numOfHearts;
    public Image[] hearts;
    public Sprite heart;

    [Header("Books Data")]
    public int numOfBooks;
    public Image[] books;
    public Sprite book;

    [Header("Player Stats Data")]
    public int health;
    public int lives;
    public int points;
    float moveInput;
    public float jumpForce;
    public float speed;
    bool isGrounded;
    
    [Header("Text Data")]
    public Text livesText;

    [Header("Particle System Data")]
    public ParticleSystem particle;
    public GameObject[] particles;

    public Transform groundCheck;
    public float checkRadius;
    public LayerMask ground;

    private bool isCollide;
    public float timer;
       

    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        points = gc.points;
        health = gc.health;
        numOfHearts = gc.health;
        numOfBooks = gc.points;
        lives = gc.lives;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {                   
        FillHearts();
        FillBooks();

        if (lives >= 0)
        {
            if (health <= 0)
            {
                lives--;
                health += 7;
            }
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
        
        livesText.text = "x" + lives;
    }

    private void FillBooks()
    {
        numOfBooks = points;
        for (int i = 0; i < books.Length; i++)
        {
            if (i < numOfBooks)
            {
                books[i].enabled = true;
            }
            else
            {
                books[i].enabled = false;
            }
        }
    }

    private void FillHearts()
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
            PlayParticle();
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
            PlayParticle();
            rb.velocity = Vector2.up * jumpForce;
            anim.SetTrigger("isJump");
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Book"))
        {
            points++;            
            if (points == 5)
            {
                print("Level Complete");
            }
            Instantiate(particles[0], col.transform.position, Quaternion.identity);
            Destroy(col.gameObject);            
        }
        if (col.CompareTag("Heart"))
        {
            Instantiate(particles[3], col.transform.position, Quaternion.identity);
            health += 1;
            if (health > 7)
            {
                lives++;
                health = 1;
            }                
            Destroy(col.gameObject);
        }
        if (col.CompareTag("Cop") && col.name != "Worker")
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
        if (col.name == "Worker")
        {
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;

                if (health >= 0 && lives > -1)
                    health -= col.GetComponent<PoliceMan>().damage;
                else
                    return;
                Instantiate(particles[4], col.transform.position, Quaternion.identity);
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
        if (col.CompareTag("FinishLevel"))
        {
            FinishLevel();
            
            
        }
        if (col.CompareTag("Death"))
        {
            lives--;
            transform.position = new Vector3(.59f, -.4f, 0f);
            transform.rotation = new Quaternion(0,0,0,0);
        }
    }

    private void FinishLevel()
    {
        if (points == 5)
        {
            points = 0;
            numOfBooks = 0;
            gc.points = 0;
            gc.sceneIndex++;
            gc.LoadNextLevel();
        }
    }

    void PlayParticle()
    {
        particle.Play();
    }
}
