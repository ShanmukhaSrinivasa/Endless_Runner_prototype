using Unity.Netcode;
using UnityEngine;

public class MultiplayerMatchManager : NetworkBehaviour
{
    public static MultiplayerMatchManager Instance;

    private float finalMatchDistance;

    private NetworkVariable<int> alivePlayers = new NetworkVariable<int>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            alivePlayers.Value = 2;
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
        }
        else
        {
            PlayerStats.AddLoss();
        }

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
}