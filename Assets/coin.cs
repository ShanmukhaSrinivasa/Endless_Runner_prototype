using UnityEngine;

public class coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<player>() != null)
        {
            GameManager.instance.coins++;
            Destroy(gameObject);
        }
    }
}
