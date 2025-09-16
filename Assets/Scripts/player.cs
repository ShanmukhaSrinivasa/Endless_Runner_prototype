using System.Collections;
using UnityEngine;

public class player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Death Info")]
    private bool isDead;
    [HideInInspector] public bool playerUnlocked;
    [HideInInspector] public bool extraLife;

    [Header("VFX")]
    [SerializeField] private ParticleSystem dustFX;
    [SerializeField] private ParticleSystem bloodSplatterFX;
    


    [Header("Knockback Info")]
    [SerializeField] private Vector2 knockBackDir;
    private bool IsKnocked;
    private bool canBeKnocked = true;

    [Header("Move Info")]
    [SerializeField] private float speedToSurvive = 18;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float MaxSpeed;
    [SerializeField] private float speedMultiplier;
    private float defaultSpeed;
    [Space]
    [SerializeField] private float milestoneIncreaser;
    private float defaultMileStoneIncreaser;
    private float speedMilestone;

    private bool readyToLand;

    [Header("Jump Info")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;


    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    [HideInInspector] public float slideCooldownCounter;
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

    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1; //Offset for position before climb
    [SerializeField] private Vector2 offset2; //Offset for position after climb

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMileStoneIncreaser = milestoneIncreaser;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorControllers();

        checkGroundCollision();

        slideTimeCount -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        extraLife = moveSpeed >= speedToSurvive;

        if (Input.GetKeyDown(KeyCode.K))
        {
            knockBack();
        }

        if (Input.GetKeyDown(KeyCode.O) && !isDead)
        {
            StartCoroutine(Death());
        }

        if (isDead)
        {
            return;
        }

        if (IsKnocked)
        {
            return;
        }

        if (playerUnlocked)
        {
            SetupMovement();
        }

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        speedController();

        CheckForLanding();
        checkForLedge();
        checkForSlideCancel();
        checkInput();
    }

    private void CheckForLanding()
    {
        if (rb.linearVelocity.y < -5 && !isGrounded)
        {
            readyToLand = true;
        }

        if (readyToLand && isGrounded)
        {
            dustFX.Play();
            readyToLand = false;
        }
    }

    public void Damage()
    {
        bloodSplatterFX.Play();

        if (extraLife)
        {
            knockBack();
        }
        else
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        AudioManager.instance.PlaySFX(4);
        isDead = true;
        canBeKnocked = false;
        rb.linearVelocity = knockBackDir;
        anim.SetBool("IsDead", true);

        Time.timeScale = .6f;

        yield return new WaitForSeconds(.5f);

        Time.timeScale = 1f;

        rb.linearVelocity = new Vector2(0, 0);

        GameManager.instance.GameEnded();
    }

    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);

        canBeKnocked = false;
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
        yield return new WaitForSeconds(.3f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.35f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.4f);

        sr.color = originalColor;
        canBeKnocked = true;
    }

    #region KnockBack
    private void knockBack()
    {
        if (!canBeKnocked)
        {
            return;
        }

        SpeedReset();
        StartCoroutine(Invincibility());
        IsKnocked = true;
        rb.linearVelocity = knockBackDir;
    }

    private void knockBackFinished()
    {
        IsKnocked = false;
    }
    #endregion KnockBack

    #region Speed Control
    private void SpeedReset()
    {
        if (IsSliding)
        {
            return;
        }

        moveSpeed = defaultSpeed; ;
        milestoneIncreaser = defaultMileStoneIncreaser;
    }

    private void speedController()
    {
        if(moveSpeed == MaxSpeed)
        {
            return;
        }

        if (transform.position.x > speedMilestone)
        {
            speedMilestone = speedMilestone + milestoneIncreaser;

            moveSpeed = moveSpeed * speedMultiplier;
            milestoneIncreaser = milestoneIncreaser * speedMultiplier;
        }

        if (moveSpeed > MaxSpeed)
        {
            moveSpeed = MaxSpeed;
        }
    }
    #endregion Speed Control

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


    private void checkForSlideCancel()
    {
        if(slideTimeCount < 0 && !ceilingDetected)
        {
            IsSliding = false;
        }
    }


    private void SetupMovement()
    {
        if (wallDetected)
        {
            SpeedReset();
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

    #region Input
    public void slidingButton()
    {
        if (isDead)
        {
            return;
        }

        if (rb.linearVelocity.x != 0 && slideCooldownCounter < 0)
        {
            dustFX.Play();
            IsSliding = true;
            slideTimeCount = slideTime;
            slideCooldownCounter = slideCooldown;
        }
    }

    public void jumpButton()
    {
        
        if(IsSliding || isDead)
        {
            return;
        }

        RollAnimFinished();

        if(isGrounded)
        {
            Jump(jumpForce);
        }
        else if(canDoubleJump)
        {
            canDoubleJump = false;
            Jump(doubleJumpForce);
        }
    }

    private void Jump(float force)
    {
        dustFX.Play();
        AudioManager.instance.PlaySFX(Random.Range(1, 2));
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
    }
    private void checkInput()
    {
        //if (Input.GetButtonDown("Fire2"))
        //{
        //    playerUnlocked = true;
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpButton();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            slidingButton();
        }
    }
    #endregion Input

    #region Animations
    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsSliding", IsSliding);
        anim.SetBool("canClimb", canClimb);
        anim.SetBool("IsKnocked", IsKnocked);

        if(rb.linearVelocity.y < -20)
        {
            anim.SetBool("canRoll", true);
        }
        
    }

    public void RollAnimFinished()
    {

        anim.SetBool("canRoll", false);
    }

    #endregion Animations


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
