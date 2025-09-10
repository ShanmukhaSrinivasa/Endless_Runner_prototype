using System;
using UnityEngine;

public class player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    private bool canDoubleJump;
    private bool playerUnlocked;

    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    private float slideCooldownCounter;
    private float slideTimeCount;
    private bool IsSliding;

    [Header("Collision Info")]
    [SerializeField] private float groundDistance;
    [SerializeField] private float ceilingCheckDistance;
    [SerializeField] private LayerMask whatsIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallSize;
    private bool isGrounded;
    private bool wallDetected;
    private bool ceilingDetected;
    [HideInInspector] public bool ledgeDetected;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorControllers();

        checkGroundCollision();

        slideTimeCount -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        if (playerUnlocked)
        {
            Movement();
        }

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        checkForSlide();

        checkInput();
    }

    private void checkForSlide()
    {
        if(slideTimeCount < 0 && !ceilingDetected)
        {
            IsSliding = false;
        }
    }


    private void Movement()
    {
        if (wallDetected)
        {
            return;
        }

        if(IsSliding)
        {
            rb.linearVelocity = new Vector2(slideSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
    }



    private void slidingButton()
    {
        if (rb.linearVelocity.x != 0 && slideCooldownCounter < 0)
        {
            IsSliding = true;
            slideTimeCount = slideTime;
            slideCooldownCounter = slideCooldown;
        }
    }

    private void jumpButton()
    {
        if(IsSliding)
        {
            return;
        }

        if(isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if(canDoubleJump)
        {
            canDoubleJump = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
        }
    }
    private void checkInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            playerUnlocked = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpButton();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            slidingButton();
        }
    }
    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsSliding", IsSliding);

        
    }

    private void checkGroundCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, whatsIsGround);
        ceilingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, whatsIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallSize, 0, Vector2.zero, 0, whatsIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceilingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallSize);
    }
}
