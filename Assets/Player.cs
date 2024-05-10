using System;
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
    [SerializeField] private float jumpForce;
    private bool canDoublejump;
    [SerializeField] private float DoubleJumpForce;
    
    
    //private bool isrunning;
    
    private bool Playerrunning;

    [Header("Slide info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    private float slideCooldownCounter;
    private float slideTimerCounter;
    private bool isSliding;

    [Header("Collision Info")]
    [SerializeField] private float groundCheckdistance;
    [SerializeField] private LayerMask whatisground;
    [SerializeField] private Transform WallCheck;
    [SerializeField] private Vector2 WallCheckSize;
    [SerializeField] private float CeilingCheckDistance;

    private bool ceilingdetected;
    private bool isgrounded;
    private bool walldetected;

    [HideInInspector] public bool ledgedetected;

    [Header("Ledge info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;

    private Vector2 climbbegunposition;
    private Vector2 climboverposition;

    private bool cangrabledge = true; //default value false
    private bool canclimb;


    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        anim= GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        Checkcollision();
        Animatorcontrollers();

        slideTimerCounter = slideTimerCounter - Time.deltaTime;
        slideCooldownCounter = slideCooldownCounter - Time.deltaTime;

        if (isgrounded)
            canDoublejump = true;

        if (Playerrunning == true)
            Movement();

        if (Playerrunning == false && isgrounded == true)
            rb.velocity = new Vector2(0, rb.velocity.y);


        CheckForSlide();
        Checkinput();
    }

    private void CheckForLedge()
    {
        if (ledgedetected == true && cangrabledge == true)
        {
            cangrabledge = false;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climbbegunposition = ledgePosition + offset1;
            climboverposition = ledgePosition + offset2;

            canclimb = true;

        }

        if (canclimb == true)
            transform.position = climbbegunposition;
    }



    private void CheckForSlide()
    {

        if (slideTimerCounter < 0 && ceilingdetected == false)
            isSliding = false;
    }

    private void Movement()
    {

        if (walldetected == true) //if theres a wall then stop taking input
            return;

        if(isSliding == true)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        else
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }




    private void SlideButton()
    {
        if (rb.velocity.x != 0 && slideCooldownCounter < 0) //Allow slide only after slidecoolsdown
        {
        isSliding = true;
        slideTimerCounter = slideTime;
            slideCooldownCounter = slideCooldown;
        }
;
    }

    private void JumpButton()
    {

        if (isSliding == true) //dont allow jump in slide
            return;

        if (isgrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (canDoublejump)
        {
            canDoublejump = false;
            rb.velocity = new Vector2(rb.velocity.x, DoubleJumpForce);
        }
    }
    private void Checkinput()
    {
        if (Input.GetButtonDown("Horizontal"))
            Playerrunning = true;

        if (Input.GetButtonUp("Horizontal"))
            Playerrunning = false;

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();

        if (Input.GetButtonDown("Jump")) // KeyCode.Space can replace with GetButton("string") -> adjust from project settings for gamepad
        {
            JumpButton();
            Debug.Log("Jump");
        }

    }

    private void Checkcollision()
    {
        isgrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckdistance, whatisground);
        walldetected = Physics2D.BoxCast(WallCheck.position, WallCheckSize, 0, Vector2.zero, 0, whatisground);
        ceilingdetected = Physics2D.Raycast(transform.position, Vector2.up, CeilingCheckDistance, whatisground);

        Debug.Log(ledgedetected); //Check if ledge collider is working properly
    }
    private void Animatorcontrollers()
    {
        //isrunning = rb.velocity.x != 0;
        //anim.SetBool("isrunning", isrunning);
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);

        anim.SetBool("isgrounded", isgrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canDoublejump", canDoublejump);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckdistance));
        Gizmos.DrawWireCube(WallCheck.position, WallCheckSize);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + CeilingCheckDistance));
    }
}