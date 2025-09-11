using UnityEngine;

public class trap : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<player>() != null)
        {
            collision.GetComponent<player>().Damage();
        }
    }
}
