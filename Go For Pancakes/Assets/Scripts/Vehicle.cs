using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle : MonoBehaviour
{
    public float speed;
    public float timer = 35f;
    void FixedUpdate()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.transform.parent = transform;
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {

            if (col.GetComponent<Player>().inputActions.Player.Jump.phase == InputActionPhase.Performed)
            {
                col.transform.parent = null;
            }
        }

    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.transform.parent = null;
        }
    }
}
