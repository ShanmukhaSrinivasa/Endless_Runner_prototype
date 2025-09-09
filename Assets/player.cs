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

    [Header("Collision Info")]
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask whatsIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallSize;
    private bool isGrounded;
    private bool wallDetected;


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

        if(isGrounded)
        {
            canDoubleJump = true;
        }

        checkInput();
    }

    private void AnimatorControllers()
    {
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        if (playerUnlocked && !wallDetected)
        {
            Movement();
        }
    }

    private void Movement()
    {
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
    }

    private void checkGroundCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, whatsIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallSize, 0, Vector2.zero, 0, whatsIsGround);
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
    }

    private void jumpButton()
    {
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallSize);
    }
}
