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
    private SpriteRenderer sr;

    private bool isDead;
    //private bool isrunning;
    private bool Playerrunning;

    [Header("Knockback info")]
    [SerializeField] private Vector2 knockbackDir;
    private bool isKnocked;
    private bool canbeKnocked = true;


    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float Maxspeed;
    [SerializeField] private float Speedmultiplier;
    private float defaultSpeed;
    [Space]
    [SerializeField] private float MilestoneIncreaser;
    private float defaultMilestoneIncrease;
    private float speedMilestone;

    [Header("Jump Info")]
    [SerializeField] private float jumpForce;
    private bool canDoublejump;
    [SerializeField] private float DoubleJumpForce;
    
    

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

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool cangrabledge = true; //default value false
    private bool canclimb;


    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        anim= GetComponent<Animator>();
        sr= GetComponent<SpriteRenderer>();
     
        
        speedMilestone = MilestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncrease = MilestoneIncreaser;

        
    }

    // Update is called once per frame
    void Update()
    {
        
        Checkcollision();
        Animatorcontrollers();
        Speedcontroller();

        slideTimerCounter = slideTimerCounter - Time.deltaTime;
        slideCooldownCounter = slideCooldownCounter - Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.K))
            Knockback();

        if (Input.GetKeyDown(KeyCode.O) && isDead == false)
            StartCoroutine(Die());

        if (isKnocked)
            return;

        if (isgrounded)
            canDoublejump = true;

        if (Playerrunning == true)
            SetupMovement();

        if (Playerrunning == false && isgrounded == true)
            rb.velocity = new Vector2(0, rb.velocity.y);

        CheckForLedge();
        CheckForSlideCancel();
        Checkinput();
    }

    private IEnumerator Die()
    {
        isDead = true;
        canbeKnocked = false;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(.5f);
        rb.velocity = new Vector2(0, 0);
    }

    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f); //oldeschool invicible blink sprite by changing alpha value

        Debug.Log("Invinci started for" + 5);
        canbeKnocked = false;
        sr.color = darkenColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.15f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.25f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.25f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.3f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.3f);

        sr.color = originalColor;

        Debug.Log("Invinci Over");
        canbeKnocked = true;
    }

    #region Knockback
    private void Knockback()
    {
        if (canbeKnocked == false)
            return;

        StartCoroutine(Invincibility());
        isKnocked = true;
        rb.velocity = knockbackDir;
    }

    private void CancelKnockback()
    {
        isKnocked = false;
    }
    #endregion

    #region Ledgeclimb
    private void CheckForLedge()
    {
        if (ledgedetected && cangrabledge)
        {
            cangrabledge = false;
            rb.gravityScale = 0;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;

            canclimb = true;

        }

        if (canclimb == true)
        {
            transform.position = climbBegunPosition;
        }
    }
    private void LedgeClimbOver()
    {
        canclimb = false;
        rb.gravityScale = 5;
        transform.position = climbOverPosition;
        Invoke("AllowLedgeGrab", .1f);
    }
    private void AllowLedgeGrab() => cangrabledge = true;
    #endregion

    #region SpeedControl
    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
        MilestoneIncreaser = defaultMilestoneIncrease;
    }
    private void Speedcontroller()
    {
        if (moveSpeed == Maxspeed)
        {
            return;
        }

        if (transform.position.x > speedMilestone)
        {
            speedMilestone = speedMilestone + MilestoneIncreaser;

            moveSpeed = moveSpeed * Speedmultiplier;
            MilestoneIncreaser = MilestoneIncreaser * Speedmultiplier;

            if (moveSpeed > Maxspeed)
            {
                moveSpeed = Maxspeed;
            }
        }
    }
    #endregion
    private void CheckForSlideCancel()
    {

        if (slideTimerCounter < 0 && ceilingdetected == false)
            isSliding = false;
    }

    private void SetupMovement()
    {
        if (walldetected == true)
        {
            SpeedReset();
        }

        if (walldetected == true) //if theres a wall then stop taking input
            return;

        if(isSliding == true)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        else
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }



    #region Inputs
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
    #endregion
    private void Checkcollision()
    {
        isgrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckdistance, whatisground);
        walldetected = Physics2D.BoxCast(WallCheck.position, WallCheckSize, 0, Vector2.zero, 0, whatisground);
        ceilingdetected = Physics2D.Raycast(transform.position, Vector2.up, CeilingCheckDistance, whatisground);

        Debug.Log(ledgedetected); //Check if ledge collider is working properly
    }

    #region Animations
    private void Animatorcontrollers()
    {
        //isrunning = rb.velocity.x != 0;
        //anim.SetBool("isrunning", isrunning);
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);

        anim.SetBool("isgrounded", isgrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canDoublejump", canDoublejump);
        anim.SetBool("canClimb", canclimb);
        anim.SetBool("isKnocked", isKnocked);

        if (rb.velocity.y < -20)
            anim.SetBool("canRoll", true);
    }
    private void RollAnimFinished() => anim.SetBool("canRoll", false);
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckdistance));
        Gizmos.DrawWireCube(WallCheck.position, WallCheckSize);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + CeilingCheckDistance));
    }
}