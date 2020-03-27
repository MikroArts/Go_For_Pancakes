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
    private bool isMovable;

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
        levelCompletion.SetActive(false);
        isMovable = true;
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
            isMovable = false;
            livesText.text = "x0";
            health = 0;
            gameObject.transform.localScale = Vector3.zero;
            StartCoroutine(PlayOhNo());                   
            return;
        }

        if (isMovable)
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

        if (isMovable)
            moveInput = Input.GetAxis("Horizontal");
        else
            moveInput = 0;

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
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded && Time.timeScale > 0)
        {
            anim.SetTrigger("isJump");
            PlayParticle();
            rb.velocity = Vector2.up * jumpForce;            
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
                col.GetComponent<AudioSource>().PlayOneShot(col.GetComponent<PoliceMan>().AudioClip);
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
                col.GetComponent<AudioSource>().PlayOneShot(col.GetComponent<PoliceMan>().AudioClip);
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
                col.GetComponent<AudioSource>().PlayOneShot(col.GetComponent<Granny>().AudioClip);
                GetComponent<AudioSource>().PlayOneShot(audioClips[2]);
            }
            
        }


        if (col.CompareTag("FinishLevel"))
        {
            isMovable = false;

            StartCoroutine(PlayTaDa());
            //isMovable = true;
        }

        if (col.CompareTag("Death"))
        {
            if (!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().PlayOneShot(audioClips[6]);
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;
                
                lives--;
                StartCoroutine(PlayLoseLife());
            }            
        }
    }
    
    private void FinishLevel()
    {
        if (points == 5)
        {
            points = 0;
            numOfBooks = 0;
            gc.points = 0;
            gc.LoadNextLevel();
        }
    }

    IEnumerator PlayTaDa()
    {

        if (!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(audioClips[4]);
        fadePanel.SetTrigger("FadeOut");
        yield return new WaitForSeconds(3.2f);        
        FinishLevel();
    }
    IEnumerator PlayOhNo()
    {
        if (!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().PlayOneShot(audioClips[6]);
        fadePanel.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2.2f);
        GetComponent<AudioSource>().Stop();
        yield return new WaitForSeconds(1f);
        gc.GameOver();
    }

    IEnumerator PlayLoseLife()
    {
        fadePanel.SetTrigger("FadeOut");        
        yield return new WaitForSeconds(1.5f);
        transform.position = new Vector3(.59f, -.4f, 0f);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    void PlayParticle()
    {
        particle.Play();
    }
}
