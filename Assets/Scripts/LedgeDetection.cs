using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private player player;
    [SerializeField] private Enemy enemy;

    [SerializeField] private PlayerSinglePlayer singlePlayer;

    //public bool ledgeDetected;

    private bool canDetected;

    private BoxCollider2D boxCd => GetComponent<BoxCollider2D>();

    private void Start()
    {
        canDetected = true;
    }

    private void FixedUpdate()
    {
        bool detected = false;

        if (canDetected)
        {
            detected = Physics2D.OverlapCircle(transform.position,radius,whatIsGround);
        }

        if (player != null)
        {
            player.ledgeDetected = detected;
        }

        if (singlePlayer != null)
        {
            singlePlayer.ledgeDetected = detected;
        }

        if (enemy != null)
        {
            enemy.ledgeDetected = detected;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetected = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCd.bounds.center, boxCd.bounds.size, 0);

        foreach (var hit in colliders)
        {
            if (hit.gameObject.GetComponent<PlatformController>() != null)
            {
                return;
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetected = true;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
