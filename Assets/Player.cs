using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Rigidbody2D rb;
    private Animator anim;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private int jumpForce;

    
    //private bool isrunning;
    
    private bool Playerrunning;


    [Header("Collision Info")]
    [SerializeField] private float groundCheckdistance;
    [SerializeField] private LayerMask whatisground;

    private bool isgrounded;

    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        anim= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Animatorcontrollers();

        if (Playerrunning == true)
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        Checkcollision();
        Checkinput();
    }

    private void Animatorcontrollers()
    {
        //isrunning = rb.velocity.x != 0;
        //anim.SetBool("isrunning", isrunning);
        anim.SetBool("isgrounded", isgrounded);
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void Checkcollision()
    {
        isgrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckdistance, whatisground);
    }

    private void Checkinput()
    {
        if (Input.GetButtonDown("Horizontal"))
            Playerrunning = true;

        if (Input.GetButtonUp("Horizontal"))
            Playerrunning = false;


        if (Input.GetButtonDown("Jump") && isgrounded == true) // KeyCode.Space can replace with GetButton("string") -> adjust from project settings for gamepad
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Debug.Log("Jump");
        }

}
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckdistance));
    }
}