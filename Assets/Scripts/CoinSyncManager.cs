using Unity.Netcode;
using UnityEngine;

public class CoinSyncManager : NetworkBehaviour
{
    public static CoinSyncManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Rpc(SendTo.Server)]
    public void CollectCoinServerRpc(Vector2 coinPosition,ulong collectorId)
    {
        RemoveCoinClientRpc(coinPosition,collectorId);
    }

    [Rpc(SendTo.Everyone)]
    private void RemoveCoinClientRpc(Vector2 coinPosition,ulong collectorId)
    {
        coin[] allCoins = FindObjectsByType<coin>(FindObjectsSortMode.None);

        foreach (coin c in allCoins)
        {
            if (Vector2.Distance(c.transform.position,coinPosition) < 0.05f)
            {
                Destroy(c.gameObject);
                break;
            }
        }

        if (NetworkManager.Singleton.LocalClientId == collectorId)
        {
            AudioManager.instance.PlaySFX(0);

            GameManager.instance.coins++;
        }
    }
}