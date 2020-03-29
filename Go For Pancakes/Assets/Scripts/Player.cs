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
    float moveInput;
    public float jumpForce;
    public float speed;
    bool isGrounded;
    
    [Header("Text Data")]
    public Text livesText;

    [Header("Particle System Data")]
    public ParticleSystem smoke;
    public ParticleSystem dieParticle;
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

    public RectTransform cloud;
    public float idleTime;
    private bool isIdle;
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();        
        numOfHearts = gc.health;
        numOfBooks = gc.points;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        levelCompletion.SetActive(false);
        isMovable = true;
        idleTime = 6;
    }
    void Update()
    {
        if (moveInput == 0 && isGrounded && !(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
        {
            idleTime -= Time.deltaTime;
            
            
            if (idleTime <= 0)
            {
                cloud.localScale = new Vector3(1, 1, 1);
                cloud.localEulerAngles = new Vector3(0, 0, 0);
            }            
        }
        else
        {
            idleTime = 5;
            cloud.localScale = Vector3.zero;
        }

        FillHearts();
        FillBooks();

        if (gc.lives >= 0)
        {
            if (gc.health <= 0)
            {
                gc.lives--;
                gc.health += 7;
            }
        }
        else
        {
            isMovable = false;
            livesText.text = "x0";
            gc.health = 0;
            gameObject.transform.localScale = new Vector3(.01f, .01f, .01f);
            dieParticle.Play();
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
        
        livesText.text = "x" + gc.lives;
    }

    private void FillBooks()
    {
        numOfBooks = gc.points;
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
        numOfHearts = gc.health;
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
            gc.points++;            
            if (gc.points == 5)
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
            gc.health += 1;
            if (gc.health > 7)
            {
                heartUI.GetComponent<Animator>().SetTrigger("isLiveUp");
                gc.lives++;
                gc.health = 1;
            }                
            Destroy(col.gameObject);
            GetComponent<AudioSource>().PlayOneShot(audioClips[1]);
        }
                
        if (col.CompareTag("Enemy"))
        {
            if (timer <= 0)
            {
                StartCoroutine(ShowEnemyCloud(col));
                isCollide = true;
                timer = 1f;

                if (gc.health >= 0 && gc.lives > -1)
                {
                    heartUI.GetComponent<Animator>().SetTrigger("isCollect");
                    gc.health -= col.GetComponent<Enemy>().damage;
                }
                else
                    return;

                Instantiate(col.GetComponent<Enemy>().particle, col.transform.position, Quaternion.identity);
                GameObject.FindGameObjectWithTag("CameraHolder").GetComponentInChildren<CameraFollow>().ShakeCam();
                col.GetComponent<AudioSource>().PlayOneShot(col.GetComponent<Enemy>().AudioClip);
                GetComponent<AudioSource>().PlayOneShot(audioClips[2]);
            }

        }


        if (col.CompareTag("Finish"))
        {
            isMovable = false;

            StartCoroutine(PlayTaDa());
        }

        if (col.CompareTag("Respawn"))
        {
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;
                
                StartCoroutine(PlayLoseLife());
            }            
        }
    }

    

    private void FinishLevel()
    {
        if (gc.points == 5)
        {
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
        yield return new WaitForSeconds(.1f);
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
        dieParticle.Play();
        if (!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(audioClips[6]);
        fadePanel.SetTrigger("FadeOut");
        gameObject.transform.localScale = new Vector3(.222f,.222f,.222f);
        yield return new WaitForSeconds(1.5f);
        gc.lives--;
        transform.position = new Vector3(.59f, -.4f, 0f);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }
    
    IEnumerator ShowEnemyCloud(Collider2D col)
    {
        col.GetComponent<Enemy>().cloud.localScale = new Vector3(1, 1, 1);
        yield return new WaitForSeconds(1f);
        col.GetComponent<Enemy>().cloud.localScale = Vector3.zero;
    }
    
    void PlayParticle()
    {
        smoke.Play();
    }
}
