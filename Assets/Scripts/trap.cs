using UnityEngine;

public class trap : MonoBehaviour
{
    [SerializeField] protected float chanceToSpawn = 60;

    protected virtual void start()
    {
        bool canSpawn = chanceToSpawn >= Random.Range(0, 100);

        if (!canSpawn)
        {
            Destroy(gameObject);
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<player>() != null)
        {
            collision.GetComponent<player>().Damage();
        }
    }
}
