using UnityEngine;

public class player : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Move Info")]
    public float moveSpeed;
    public float jumpForce;
    private bool begin_Run;

    [Header("Collision Info")]
    public float groundDistance;
    public LayerMask whatsIsGround;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (begin_Run == true)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }

        checkGroundCollision();

        checkInput();
    }

    private void checkGroundCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, whatsIsGround);
    }

    private void checkInput()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            begin_Run = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundDistance));
    }
}
