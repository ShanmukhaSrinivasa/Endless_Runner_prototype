using UnityEngine;

public class coin : MonoBehaviour
{
    private bool collected;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            Destroy(gameObject);
        }

        if (collision.GetComponent<PlayerSinglePlayer>() != null)
        {
            AudioManager.instance.PlaySFX(0);

            GameManager.instance.coins++;

            Destroy(gameObject);
        }

        player networkPlayer = collision.GetComponent<player>();

        if (networkPlayer != null)
        {
            if (!networkPlayer.IsOwner)
                return;

            if (collected)
                return;

            collected = true;

            CoinSyncManager.Instance.CollectCoinServerRpc(transform.position,networkPlayer.OwnerClientId);
        }
    }
}
