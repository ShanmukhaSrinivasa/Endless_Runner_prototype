using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class MultiplayerLobbyManager : MonoBehaviour
{
    public static MultiplayerLobbyManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TMP_InputField roomCodeInput;

    [Header("Relay")]
    private string relayJoinCode;

    private Allocation hostAllocation;
    private Lobby currentLobby;

    private void Awake()
    {
        Instance = this;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("CLIENT CONNECTED: " + clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("CLIENT DISCONNECTED: " + clientId);
    }

    public async void CreateRoom()
    {
        try
        {
            await CreateRelay();

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                    }
                }
            };
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("PixelDashRoom", 5, options);


            Debug.Log("Lobby created!");
            Debug.Log("Lobby Code: " +  currentLobby.LobbyCode);

            if (roomCodeText != null)
            {
                roomCodeText.text = "Room Code: " + currentLobby.LobbyCode;
            }

            ConfigureHostRelay();
            StartHost();
        }
        catch(System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinRoom()
    {
        try
        {
            string roomCode = roomCodeInput.text;

            Debug.Log("Trying to join: " + roomCode);

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(roomCode);

            if (currentLobby.Data.ContainsKey("RelayCode"))
            {
                Debug.Log("Relay Code Found!");

                string relayCode = currentLobby.Data["RelayCode"].Value;

                Debug.Log("Relay Code: " + relayCode);

                await JoinRelay(relayCode);
            }
            else
            {
                Debug.Log("Relay Code Missing");
            }

            Debug.Log("Joined Lobby!");
            Debug.Log("Players In Lobby: " + currentLobby.Players.Count);
            Debug.Log("Lobby Name: " + currentLobby.Name);
            Debug.Log("Lobby Code: " + currentLobby.LobbyCode);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async Task CreateRelay()
    {
        try
        {
            hostAllocation = await RelayService.Instance.CreateAllocationAsync(4);

            relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

            Debug.Log("Relay Created!");
            Debug.Log("Relay Join Code: " + relayJoinCode);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async Task JoinRelay(string relayCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,

                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            Debug.Log("CLIENT RELAY CONFIGURED");

            NetworkManager.Singleton.StartClient();

            Debug.Log("CLIENT STARTED");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();

        Debug.Log("HOST STARTED");
    }

    private void ConfigureHostRelay()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetHostRelayData(
            hostAllocation.RelayServer.IpV4,
            (ushort)hostAllocation.RelayServer.Port,
            hostAllocation.AllocationIdBytes,
            hostAllocation.Key,
            hostAllocation.ConnectionData
            );

        Debug.Log("HOST RELAY CONFIGURED");
    }

    public string GetLobbyCode()
    {
        if (currentLobby == null)
            return "";

        return currentLobby.LobbyCode;
    }
}
