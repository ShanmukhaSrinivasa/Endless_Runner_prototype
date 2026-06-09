using UnityEngine;

public class trap : MonoBehaviour
{
    [SerializeField] protected float chanceToSpawn = 60;

    protected virtual void Start()
    {
        bool canSpawn = chanceToSpawn >= Random.Range(0, 100);

        if (!canSpawn)
        {
            Destroy(gameObject);
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player networkPlayer = collision.GetComponent<player>();

        if (networkPlayer != null)
        {
            networkPlayer.Damage();
        }

        PlayerSinglePlayer singlePlayer =
            collision.GetComponent<PlayerSinglePlayer>();

        if (singlePlayer != null)
        {
            singlePlayer.Damage();
        }
    }
}
