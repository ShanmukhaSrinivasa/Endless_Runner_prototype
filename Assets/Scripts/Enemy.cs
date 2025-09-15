using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private player player;

    [Header("Movement Details")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float distanceToRun;
    private float maxDistance;

    [Header("Collision Info")]
    [SerializeField] private float groundDistance;
    [SerializeField] private float ceilingCheckDistance;
    [SerializeField] private LayerMask whatsIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundForwardCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallSize;
    private bool isGrounded;
    private bool groundForward;
    private bool wallDetected;
    private bool ceilingDetected;
    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1; //Offset for position before climb
    [SerializeField] private Vector2 offset2; //Offset for position after climb

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;

    private bool justRespawned = true;

    private float defaultGravityScale;

    public bool canMove;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameManager.instance.player;

        rb.linearVelocity = new Vector2(0, 0);
        defaultGravityScale = rb.gravityScale;

        rb.gravityScale = rb.gravityScale * .6f;

        maxDistance = transform.position.x + distanceToRun;
    }

    private void Update()
    {
        if (justRespawned)
        {
            if (rb.linearVelocity.y < 0)
            {
                rb.gravityScale = defaultGravityScale * 2;
            }

            if(isGrounded)
            {
                rb.linearVelocity = new Vector2(0, 0);
            }
        }

        checkGroundCollision();
        AnimationController();
        Movement();
        checkForLedge();
        SpeedController();

        if (transform.position.x > maxDistance)
        {
            canMove = false;
            return;
        }


        if (!groundForward || wallDetected)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void SpeedController()
    {
        bool playerAhead = player.transform.position.x > transform.position.x;
        bool playerFarAway = Vector2.Distance(player.transform.position, transform.position) > 2.5f;

        if (playerAhead)
        {
            if (playerFarAway)
            {
                moveSpeed = 22;
            }
            else
            {
                moveSpeed = 16;
            }
        }
        else
        {
            if (playerFarAway)
            {
                moveSpeed = 11;
            }
            else
            {
                moveSpeed = 14;
            }
        }
    }

    private void Movement()
    {
        if (justRespawned)
        {
            return;
        }

        if (canMove)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
    }

    #region Ledge Grab
    private void checkForLedge()
    {
        if (ledgeDetected && canGrabLedge)
        {
            canGrabLedge = false;
            rb.gravityScale = 0;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;

            canClimb = true;
        }

        if (canClimb)
        {
            transform.position = climbBegunPosition;
        }
    }

    private void LedgeClimbOver()
    {
        canClimb = false;
        rb.gravityScale = 6;
        transform.position = climbOverPosition;
        Invoke("AllowLedgeGrab", .1f);
    }

    private void AllowLedgeGrab()
    {
        canGrabLedge = true;
    }
    #endregion Ledge Grab

    private void AnimationController()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("canClimb", canClimb);
        anim.SetBool("justRespawned", justRespawned);
    }

    private void AnimationTrigger()
    {
        rb.gravityScale = defaultGravityScale;
        justRespawned = false;
        canMove = true;
    }


    private void checkGroundCollision()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundDistance, whatsIsGround);
        groundForward = Physics2D.Raycast(groundForwardCheck.position, Vector2.down, groundDistance, whatsIsGround);
        ceilingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, whatsIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallSize, 0, Vector2.zero, 0, whatsIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundDistance));
        Gizmos.DrawLine(groundForwardCheck.position, new Vector2(groundForwardCheck.position.x, groundForwardCheck.position.y - groundDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceilingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallSize);
    }
}
