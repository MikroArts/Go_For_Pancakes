using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform target;
    public float smoothSpeed = 2f;
    Animator anim;
    public Vector3 offset;
    Player player;
    void Start()
    {
        player = GameObject.Find("Sole").GetComponent<Player>();

        anim = GetComponentInChildren<Animator>();
        if (GameObject.FindGameObjectWithTag("Player"))
            target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void FixedUpdate()
    {
        if (target)
        {
            Vector2 smoothPos = Vector2.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
            transform.position = smoothPos;
        }

        if (player.inputActions.Player.Move.ReadValue<Vector2>().y < 0)
        {
            offset = new Vector3(0, -3f, 0);
        }
        else
        {
            if (player.inputActions.Player.Move.ReadValue<Vector2>().x > 0)
                offset = new Vector3(1f, 1.5f, 0);
            else if (player.inputActions.Player.Move.ReadValue<Vector2>().x < 0)
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
