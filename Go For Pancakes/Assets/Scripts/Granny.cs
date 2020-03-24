using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granny : MonoBehaviour
{
    public int damage;
    public float speed;
    public float distance;
    bool moveRight = true;
    
    public Transform groundCheck;
    Animator anim;

    void Start()
    {        
        anim = GetComponent<Animator>();
        anim.SetBool("isWalk", true);
    }
     
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, distance);
        if (hit.collider == false)
        {
            if (moveRight)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                moveRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                moveRight = true;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Cop") || col.CompareTag("Untagged") || col.CompareTag("Granny"))
        {
            if (moveRight)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                moveRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                moveRight = true;
            }
        }
    }
}
