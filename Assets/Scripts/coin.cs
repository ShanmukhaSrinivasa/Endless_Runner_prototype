using UnityEngine;

public class coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            Destroy(gameObject);
        }

        if (collision.GetComponent<player>() != null)
        {
            AudioManager.instance.PlaySFX(0);
            GameManager.instance.coins++;
            Destroy(gameObject);
        }
    }
}
