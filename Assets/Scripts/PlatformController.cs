using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private SpriteRenderer headerSr;

    private void Start()
    {
        headerSr.transform.parent = transform.parent;
        headerSr.transform.localScale = new Vector2(sr.bounds.size.x, .2f);
        headerSr.transform.position = new Vector2(transform.position.x, sr.bounds.max.y);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<player>() != null)
        {
            if (GameManager.instance.colorEntirePlatform)
            {
                headerSr.color = GameManager.instance.platformColor;
                sr.color = GameManager.instance.platformColor;
            }
            else
            {
                headerSr.color = GameManager.instance.platformColor;
            }
        }

    }
}
