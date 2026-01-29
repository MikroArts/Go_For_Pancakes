using System.Collections;
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
    private float currentInput = 0f;
    private float currentVelocity = 0f;
    private float smoothTime = 0.0f;

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

    public PlayerInput inputActions;
    void Start()
    {
        gc = GameController.gameController;
        numOfHearts = gc.health;
        numOfBooks = gc.points;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        levelCompletion.SetActive(false);
        isMovable = true;
        idleTime = 6;
    }
    private void Awake()
    {
        inputActions = new PlayerInput();
        inputActions.Player.Enable();
    }
    void Update()
    {
        Vector2 moveVector = inputActions.Player.Move.ReadValue<Vector2>();

        if (moveInput == 0 && isGrounded && !(moveVector.y < 0))
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
            livesText.text = "h0";
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

        livesText.text = "h" + gc.lives;

        if (inputActions.Player.Pause.WasPressedThisFrame())
        {
            GameController.gameController.Pause();
        }
    }

    private void FillBooks()
    {
        numOfBooks = gc.points;
        for (int i = 0; i < books.Length; i++)
        {
            books[i].enabled = (i < numOfBooks) ? true : false;
        }
    }

    private void FillHearts()
    {
        numOfHearts = gc.health;
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = (i < numOfHearts) ? true : false;
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        print(currentInput);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
        float targetInput = inputActions.Player.Move.ReadValue<Vector2>().x;
        currentInput = Mathf.SmoothDamp(currentInput, targetInput, ref currentVelocity, smoothTime);
        moveInput = (isMovable) ? currentInput/* Input.GetAxis("Horizontal")*/ : 0;

        if (moveInput == 0)
        {
            anim.SetBool("isRide", false);
            trotinet.GetComponent<AudioSource>().Stop();
        }
        else
        {
            PlayParticle();
            rb.linearVelocity = new Vector2(moveInput * speed * Time.deltaTime, rb.linearVelocity.y);
            anim.SetBool("isRide", true);

            if (!trotinet.GetComponent<AudioSource>().isPlaying)
                trotinet.GetComponent<AudioSource>().PlayOneShot(trotinetSound);

            transform.eulerAngles = (moveInput < 0) ? new Vector3(transform.rotation.x, 180f, transform.rotation.z) : new Vector3(transform.rotation.x, 0, transform.rotation.z);
        }
    }
    private void Jump()
    {
        if ((inputActions.Player.Jump.WasPerformedThisFrame()) && isGrounded && Time.timeScale > 0)
        {
            anim.SetTrigger("isJump");
            PlayParticle();
            rb.linearVelocity = Vector2.up * jumpForce;
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
            Instantiate(particles[6], col.transform.position, Quaternion.identity);
            Destroy(col.gameObject);
            isMovable = false;

            StartCoroutine(PlayTaDa());
        }

        if (col.CompareTag("Respawn"))
        {
            if (timer <= 0)
            {
                isCollide = true;
                timer = 1f;
                gc.health -= 5;
                heartUI.GetComponent<Animator>().SetTrigger("isCollect");
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

        if (gc.unlockedLevels == 4)
            gc.unlockedLevels = 4;
        else
            gc.unlockedLevels++;
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
        gameObject.transform.localScale = new Vector3(.222f, .222f, .222f);
        yield return new WaitForSeconds(2f);
        //gc.lives--;

        if (gc.lives < 0)
            gc.GameOver();
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

    private void OnDestroy()
    {
        inputActions.Player.Disable();
    }
}
