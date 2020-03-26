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
    public GameObject heartUI;

    [Header("Books Data")]
    public int numOfBooks;
    public Image[] books;
    public Sprite book;
    public GameObject bookUI;

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

    [Header("Audio Data")]
    public AudioClip[] audioClips;
    public AudioClip trotinetSound;
    public GameObject trotinet;

    public Animator fadePanel;

    public Transform groundCheck;
    public float checkRadius;
    public LayerMask ground;

    private bool isCollide;
    public float timer;
    public GameObject levelCompletion;

    private bool isUnmovable;

    void Start()
    {
        isUnmovable = false;
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        points = gc.points;
        health = gc.health;
        numOfHearts = gc.health;
        numOfBooks = gc.points;
        lives = gc.lives;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        levelCompletion.SetActive(false);
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
            isUnmovable = false;
        }
        else
        {
            isUnmovable = true;
            livesText.text = "x0";
            health = 0;
            fadePanel.SetTrigger("FadeOut");
            StartCoroutine(PlayOhNo());
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
        if (!isUnmovable)
            Move();
        else
            anim.SetBool("isRide", false);
    }

    private void Move()
    { 
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
        moveInput = Input.GetAxis("Horizontal");

        if (moveInput == 0)
        {
            anim.SetBool("isRide", false);
            trotinet.GetComponent<AudioSource>().Stop();
        }
        else
        {
            PlayParticle();
            rb.velocity = new Vector2(moveInput * speed * Time.deltaTime, rb.velocity.y);            
            anim.SetBool("isRide", true);

            if (!trotinet.GetComponent<AudioSource>().isPlaying)
                trotinet.GetComponent<AudioSource>().PlayOneShot(trotinetSound);

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
            GetComponent<AudioSource>().PlayOneShot(audioClips[3]);
        }        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Book"))
        {
            bookUI.GetComponent<Animator>().SetTrigger("isCollect");
            points++;            
            if (points == 5)
            {
                GetComponent<AudioSource>().PlayOneShot(audioClips[5]);
                levelCompletion.SetActive(true);
            }
            Instantiate(particles[0], col.transform.position, Quaternion.identity);
            Destroy(col.gameObject);
            GetComponent<AudioSource>().PlayOneShot(audioClips[0]);
        }


        if (col.CompareTag("Heart"))
        {
            heartUI.GetComponent<Animator>().SetTrigger("isCollect");
            Instantiate(particles[3], col.transform.position, Quaternion.identity);
            health += 1;
            if (health > 7)
            {
                heartUI.GetComponent<Animator>().SetTrigger("isLiveUp");
                lives++;
                health = 1;
            }                
            Destroy(col.gameObject);
            GetComponent<AudioSource>().PlayOneShot(audioClips[1]);
        }


        if (col.CompareTag("Cop") && col.name != "Worker")
        {
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;

                if (health >= 0 && lives > -1)
                {
                    heartUI.GetComponent<Animator>().SetTrigger("isCollect");
                    health -= col.GetComponent<PoliceMan>().damage;
                }
                else
                    return;
                Instantiate(particles[1], col.transform.position, Quaternion.identity);
                GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraFollow>().ShakeCam();
                col.GetComponent<AudioSource>().Play(0);
                GetComponent<AudioSource>().PlayOneShot(audioClips[2]);
            }            
        }


        if (col.name == "Worker")
        {
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;

                if (health >= 0 && lives > -1)
                {
                    health -= col.GetComponent<PoliceMan>().damage;
                    heartUI.GetComponent<Animator>().SetTrigger("isCollect");
                }
                else
                    return;
                Instantiate(particles[4], col.transform.position, Quaternion.identity);
                GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraFollow>().ShakeCam();
                GetComponent<AudioSource>().PlayOneShot(audioClips[2]);
            }
        }


        if (col.CompareTag("Granny"))
        {
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;

                if (health >= 0 && lives > -1)
                {
                    heartUI.GetComponent<Animator>().SetTrigger("isCollect");
                    health -= col.GetComponent<Granny>().damage;
                }
                else
                    return;

                Instantiate(particles[2], col.transform.position, Quaternion.identity);
                GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraFollow>().ShakeCam();
                col.GetComponent<AudioSource>().Play(0);
                GetComponent<AudioSource>().PlayOneShot(audioClips[2]);
            }
            
        }


        if (col.CompareTag("FinishLevel"))
        {
            isUnmovable = true;
            StartCoroutine(PlayTaDa());   
        }

        if (col.CompareTag("Death"))
        {
            lives--;
            transform.position = new Vector3(.59f, -.4f, 0f);
            transform.rotation = new Quaternion(0, 0, 0, 0);
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

    IEnumerator PlayTaDa()
    {
        if(!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().PlayOneShot(audioClips[4]);
        fadePanel.SetTrigger("FadeOut");
        yield return new WaitForSeconds(3.2f);
        FinishLevel();

    }
    IEnumerator PlayOhNo()
    {
        //if (!GetComponent<AudioSource>().isPlaying)
        //    GetComponent<AudioSource>().PlayOneShot(audioClips[5]);  --- Play death sound
        yield return new WaitForSeconds(3.2f);
        gc.GameOver();
    }

    void PlayParticle()
    {
        particle.Play();
    }
}
