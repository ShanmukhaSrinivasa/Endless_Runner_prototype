using Unity.Netcode;
using UnityEngine;

public class MultiplayerMatchManager : NetworkBehaviour
{
    public static MultiplayerMatchManager Instance;

    private float finalMatchDistance;

    private NetworkVariable<int> worldSeed = new NetworkVariable<int>();

    private NetworkVariable<int> alivePlayers = new NetworkVariable<int>();

    private const int WIN_COINS = 100;
    private const int LOSS_COINS = 25;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            alivePlayers.Value = 2;

            worldSeed.Value =Random.Range(100000,999999);

            Debug.Log("WORLD SEED = " +worldSeed.Value);

            Debug.Log("MATCH STARTED WITH " +alivePlayers.Value +" PLAYERS");
        }
    }

    [Rpc(SendTo.Server)]
    public void PlayerDiedServerRpc(ulong deadClientId)
    {
        finalMatchDistance = GameManager.instance.distance;
        alivePlayers.Value--;

        Debug.Log("PLAYER DIED: " + deadClientId);
        Debug.Log("ALIVE PLAYERS NOW = " +alivePlayers.Value);

        if (alivePlayers.Value == 1)
        {
            ulong winnerClientId = 999;

            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                if (client.Key != deadClientId)
                {
                    winnerClientId = client.Key;
                    break;
                }
            }

            Debug.Log("DECLARING WINNER");
            DeclareWinnerClientRpc(winnerClientId, finalMatchDistance);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void DeclareWinnerClientRpc(ulong winnerClientId, float matchDistance)
    {
        bool isWinner = NetworkManager.Singleton.LocalClientId == winnerClientId;

        if (isWinner)
        {
            PlayerStats.AddWin();

            int coins =PlayerPrefs.GetInt("Coins", 0);

            PlayerPrefs.SetInt("Coins",coins + WIN_COINS);
        }
        else
        {
            PlayerStats.AddLoss();

            int coins =PlayerPrefs.GetInt("Coins", 0);

            PlayerPrefs.SetInt("Coins",coins + LOSS_COINS);
        }

        PlayerPrefs.Save();

        PlayerStats.CheckBestDistance(matchDistance);

        Debug.Log("WINNER CLIENT = " + winnerClientId + " | IS WINNER = " + isWinner);

        string winnerName;

        if (winnerClientId == 0)
        {
            winnerName = MultiplayerLobbyManager.Instance.HostPlayerName;
        }
        else
        {
            winnerName = MultiplayerLobbyManager.Instance.GuestPlayerName;
        }

        GameManager.instance.ui.OpenMultiplayerResultUI(isWinner,winnerName,matchDistance);

        Time.timeScale = 0f;
    }

    public int GetWorldSeed()
    {
        return worldSeed.Value;
    }
}