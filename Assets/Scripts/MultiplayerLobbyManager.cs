using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using UnityEngine;

public class MultiplayerLobbyManager : NetworkBehaviour
{
    public static MultiplayerLobbyManager Instance;

    [Header("UI")]
    [SerializeField] private TMP_InputField roomCodeInput;

    [Header("Relay")]
    private string relayJoinCode;

    private Allocation hostAllocation;
    private Lobby currentLobby;

    private float lobbyRefreshTimer;
    private const float LOBBY_REFRESH_INTERVAL = 1f;

    public string HostPlayerName { get; private set; }
    public string GuestPlayerName { get; private set; }

    private bool localReady;
    private bool cancelSearch;
    private bool quickMatchCountdownStarted;

    private float waitingForOpponentTimer;
    private bool waitingTimeoutShown;
    private bool isLeavingLobby;

    public bool IsQuickMatch { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("LobbyManager Start");

        Debug.Log("NetworkManager = " + NetworkManager.Singleton);

        if (NetworkManager.Singleton != null)
        {
            Debug.Log("REGISTERING CALLBACKS");

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void Update()
    {
        if (currentLobby == null)
            return;

        lobbyRefreshTimer -= Time.deltaTime;

        if (lobbyRefreshTimer <= 0f)
        {
            lobbyRefreshTimer = LOBBY_REFRESH_INTERVAL;

            RefreshLobby();
        }

        if (IsQuickMatch && currentLobby.Players.Count < 2 && !waitingTimeoutShown)
        {
            waitingForOpponentTimer -= Time.deltaTime;

            if (GameManager.instance != null && GameManager.instance.ui != null)
            {
                GameManager.instance.ui.UpdateWaitingTimer(Mathf.CeilToInt(waitingForOpponentTimer));
            }

            if (waitingForOpponentTimer <= 0 && !waitingTimeoutShown)
            {
                waitingTimeoutShown = true;

                GameManager.instance.ui.ShowQuickMatchTimeout();
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("CLIENT CONNECTED CALLBACK FIRED");
        Debug.Log("CLIENT CONNECTED: " + clientId);

        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        Debug.Log("CONNECTED CLIENTS = " + playerCount);

        if (GameManager.instance != null && GameManager.instance.ui != null)
        {
            GameManager.instance.ui.UpdatePlayerCount(playerCount);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("CLIENT DISCONNECTED: " + clientId);

        if (GameManager.instance == null)
            return;

        if (GameManager.instance.currentGameMode != GameMode.Multiplayer)
            return;

        HandleDisconnect();
    }

    private async void HandleDisconnect()
    {
        if (GameManager.instance != null && GameManager.instance.ui != null)
        {
            GameManager.instance.ui.ShowDisconnectMessage("Opponent Disconnected\nReturning To Menu...");
        }

        await Task.Delay(3000);

        LeaveLobby();
    }

    public async void CreateRoom()
    {
        if (!CheckInternet())
            return;

        IsQuickMatch = false;

        quickMatchCountdownStarted = false;

        GameManager.instance.currentGameMode =GameMode.Multiplayer;

        GameManager.instance.ui.OpenSearchingPanel("Creating Room...");

        try
        {
            await CreateRelay();

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "LobbyType",new DataObject(DataObject.VisibilityOptions.Public,"Private")
                    },

                    {
                        "Status",new DataObject(DataObject.VisibilityOptions.Public,"Waiting")
                    },

                    {
                        "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                    },

                    {
                        "HostName",new DataObject(DataObject.VisibilityOptions.Public,LoginManager.Instance.GetPlayerName())
                    }
                }
            };
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("PixelDashRoom",5,options);

            lobbyRefreshTimer = LOBBY_REFRESH_INTERVAL;
            isLeavingLobby = false;

            await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id,AuthenticationService.Instance.PlayerId,new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "Ready",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"False")
                    }
                }
            });

            Debug.Log("Lobby created!");
            Debug.Log("Lobby Code: " + currentLobby.LobbyCode);

            ConfigureHostRelay();
            StartHost();

            GameManager.instance.ui.CloseSearchingPanel();

            GameManager.instance.ui.OpenMultiplayerLobby();

            GameManager.instance.ui.ConfigureLobbyUI(false);

            GameManager.instance.ui.SetLobbyRoomCode(currentLobby.LobbyCode);
        }
        catch(System.Exception e)
        {
            GameManager.instance.ui.CloseSearchingPanel();

            Debug.LogError(e);
        }
    }

    public async void JoinRoom()
    {
        if (!CheckInternet())
            return;

        GameManager.instance.currentGameMode = GameMode.Multiplayer;

        try
        {
            string roomCode = roomCodeInput.text;

            Debug.Log("Trying to join: " + roomCode);

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(roomCode);

            lobbyRefreshTimer = LOBBY_REFRESH_INTERVAL;
            isLeavingLobby = false;

            await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id,AuthenticationService.Instance.PlayerId,new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,LoginManager.Instance.GetPlayerName())
                    },

                    {
                        "Ready",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"False")
                    }
                }
            });

            GameManager.instance.ui.OpenMultiplayerLobby();

            GameManager.instance.ui.ConfigureLobbyUI(false);

            GameManager.instance.ui.SetLobbyRoomCode(currentLobby.LobbyCode);

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

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        base.OnDestroy();
    }

    [Rpc(SendTo.Server)]
    public void StartMatchRpc()
    {
        Debug.Log("HOST REQUESTED MATCH START");

        SetLobbyStatusPlaying();

        StartMatchClientRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void StartMatchClientRpc()
    {
        Debug.Log("MATCH STARTED ON CLIENT");

        GameManager.instance.currentGameMode =GameMode.Multiplayer;

        GameManager.instance.ui.OpenInGameUI();

        GameManager.instance.BeginGamePlay();
    }

    private bool refreshingLobby;

    private async void RefreshLobby()
    {
        if (refreshingLobby)
            return;

        if (isLeavingLobby)
            return;

        if (currentLobby == null)
            return;

        refreshingLobby = true;

        string lobbyId = currentLobby.Id;

        try
        {
            Debug.Log("Refreshing Lobby: " + lobbyId);

            Lobby refreshedLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);

            if (refreshedLobby == null)
            {
                Debug.LogWarning("Lobby returned null");
                return;
            }

            currentLobby = refreshedLobby;

            if (GameManager.instance == null)
                return;

            if (GameManager.instance.ui == null)
                return;

            GameManager.instance.ui.UpdatePlayerCount(currentLobby.Players.Count);

            HostPlayerName = "Host";

            if (currentLobby.Data.ContainsKey("HostName"))
            {
                HostPlayerName = currentLobby.Data["HostName"].Value;
            }

            GuestPlayerName = "";

            if (currentLobby.Players.Count > 1)
            {
                Player guestPlayer = currentLobby.Players[1];

                if (guestPlayer.Data != null && guestPlayer.Data.ContainsKey("PlayerName"))
                {
                    GuestPlayerName = guestPlayer.Data["PlayerName"].Value;
                }
            }

            GameManager.instance.ui.UpdatePlayerNames(HostPlayerName, GuestPlayerName);

            if (IsQuickMatch && currentLobby.Players.Count >= 2 && !quickMatchCountdownStarted)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    ShowMatchFoundClientRpc();

                    StartQuickMatchCountdown();
                }
            }

            bool hostReady = false;
            bool guestReady = false;

            if (currentLobby.Players.Count > 0)
            {
                Player hostPlayer = currentLobby.Players[0];

                if (hostPlayer.Data != null && hostPlayer.Data.ContainsKey("Ready"))
                {
                    bool.TryParse(hostPlayer.Data["Ready"].Value, out hostReady);
                }
            }

            if (currentLobby.Players.Count > 1)
            {
                Player guestPlayer = currentLobby.Players[1];

                if (guestPlayer.Data != null && guestPlayer.Data.ContainsKey("Ready"))
                {
                    bool.TryParse(guestPlayer.Data["Ready"].Value, out guestReady);
                }
            }

            GameManager.instance.ui.UpdatePlayerNames(HostPlayerName, GuestPlayerName);

            GameManager.instance.ui.UpdateReadyStatus(hostReady, guestReady);

            GameManager.instance.ui.RefreshHostUI();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning("Lobby refresh failed: " + e.Message);

            currentLobby = null;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            refreshingLobby = false;
        }
    }

    public async void LeaveLobby()
    {
        isLeavingLobby = true;

        try
        {
            if (currentLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id,AuthenticationService.Instance.PlayerId);
            }

            cancelSearch = true;
            waitingTimeoutShown = true;

            currentLobby = null;

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
            }

            Time.timeScale = 1f;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Endless_Runner");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async void ToggleReady()
    {
        localReady = !localReady;

        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id,AuthenticationService.Instance.PlayerId,new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            "Ready",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,localReady.ToString())
                        }
                    }
                });
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async void CreatePublicRoom()
    {
        quickMatchCountdownStarted = false;

        waitingForOpponentTimer = 30f;
        waitingTimeoutShown = false;

        GameManager.instance.currentGameMode = GameMode.Multiplayer;

        try
        {
            await CreateRelay();

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "LobbyType",new DataObject(DataObject.VisibilityOptions.Public,"Public")
                    },

                    {
                        "Status",new DataObject(DataObject.VisibilityOptions.Public,"Waiting")
                    },

                    {
                        "RelayCode",new DataObject(DataObject.VisibilityOptions.Member,relayJoinCode)
                    },

                    {
                        "HostName",new DataObject(DataObject.VisibilityOptions.Public,LoginManager.Instance.GetPlayerName())
                    }
                }
            };

            currentLobby =await LobbyService.Instance.CreateLobbyAsync("QuickMatch_" +Random.Range(1000, 9999),2,options);

            lobbyRefreshTimer = LOBBY_REFRESH_INTERVAL;
            isLeavingLobby = false;

            await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id,AuthenticationService.Instance.PlayerId,new UpdatePlayerOptions
                    {
                        Data =new Dictionary<string,PlayerDataObject>
                        {
                            {
                                "Ready",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"False")
                            }
                        }
                    });

            ConfigureHostRelay();
            StartHost();

            GameManager.instance.ui.CloseSearchingPanel();

            GameManager.instance.ui.OpenMultiplayerLobby();

            GameManager.instance.ui.ConfigureLobbyUI(true);

            GameManager.instance.ui.SetLobbyRoomCode(currentLobby.LobbyCode);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async void QuickMatch()
    {
        if (!CheckInternet())
            return;

        IsQuickMatch = true;

        cancelSearch = false;

        GameManager.instance.ui.OpenSearchingPanel("Searching For Opponent...");
        await Task.Delay(500);

        try
        {
            QueryResponse response =await LobbyService.Instance.QueryLobbiesAsync();

            if (cancelSearch)
                return;

            foreach (Lobby lobby in response.Results)
            {
                if (lobby.AvailableSlots <= 0)
                    continue;

                if (!lobby.Data.ContainsKey("LobbyType"))
                    continue;

                if (!lobby.Data.ContainsKey("Status"))
                    continue;

                if (lobby.Data["LobbyType"].Value!= "Public")
                    continue;

                if (lobby.Data["Status"].Value!= "Waiting")
                    continue;

                Debug.Log("Found Public Lobby");

                currentLobby =await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

                lobbyRefreshTimer = LOBBY_REFRESH_INTERVAL;
                isLeavingLobby = false;

                IsQuickMatch = true;

                await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id,AuthenticationService.Instance.PlayerId,new UpdatePlayerOptions
                        {
                            Data =new Dictionary<string,PlayerDataObject>
                            {
                                {
                                    "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,LoginManager.Instance.GetPlayerName())
                                },

                                {
                                    "Ready",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"False")
                                }
                            }
                        });

                GameManager.instance.ui.CloseSearchingPanel();

                GameManager.instance.ui.OpenMultiplayerLobby();

                GameManager.instance.ui.ConfigureLobbyUI(true);

                GameManager.instance.ui.SetLobbyRoomCode(currentLobby.LobbyCode);

                string relayCode =currentLobby.Data["RelayCode"].Value;

                await JoinRelay(relayCode);

                return;
            }

            Debug.Log("No Public Lobby Found");

            await Task.Delay(Random.Range(500, 1500));

            response = await LobbyService.Instance.QueryLobbiesAsync();

            foreach (Lobby lobby in response.Results)
            {
                if (lobby.AvailableSlots <= 0)
                {
                    continue;
                }

                if (!lobby.Data.ContainsKey("LobbyType"))
                {
                    continue;
                }

                if (!lobby.Data.ContainsKey("Status"))
                {
                    continue;
                }

                if (lobby.Data["LobbyType"].Value != "Public")
                {
                    continue;
                }

                if (lobby.Data["Status"].Value != "Waiting")
                {
                    continue;
                }

                Debug.Log("Found Lobby After Retry");

                currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

                lobbyRefreshTimer = LOBBY_REFRESH_INTERVAL;
                isLeavingLobby = false;

                waitingTimeoutShown = false;

                string relayCode = currentLobby.Data["RelayCode"].Value;

                await JoinRelay(relayCode);

                return;
            }

            CreatePublicRoom();
        }
        catch (System.Exception e)
        {
            GameManager.instance.ui.CloseSearchingPanel();
            Debug.LogError(e);
        }
    }

    private async void SetLobbyStatusPlaying()
    {
        if (currentLobby == null)
            return;

        try
        {
            await LobbyService.Instance
                .UpdateLobbyAsync(currentLobby.Id,new UpdateLobbyOptions
                    {
                        Data =new Dictionary<string,DataObject>
                        {
                            {
                                "Status",new DataObject(DataObject.VisibilityOptions.Public,"Playing")
                            }
                        }
                    });
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void ContinueSearching()
    {
        waitingForOpponentTimer = 30f;
        waitingTimeoutShown = false;

        GameManager.instance.ui.UpdateWaitingTimer(30);
        GameManager.instance.ui.HideQuickMatchTimeout();
    }

    public void CancelQuickMatchLobby()
    {
        LeaveLobby();
    }

    public void CancelQuickMatch()
    {
        cancelSearch = true;

        GameManager.instance.ui.CloseSearchingPanel();
    }

    private async void StartQuickMatchCountdown()
    {
        if (quickMatchCountdownStarted)
            return;

        quickMatchCountdownStarted = true;

        for (int i = 5; i > 0; i--)
        {
            UpdateCountdownClientRpc(i);

            await Task.Delay(1000);
        }

        if (NetworkManager.Singleton != null &&NetworkManager.Singleton.IsHost)
        {
            StartMatchRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateCountdownClientRpc(int seconds)
    {
        if (GameManager.instance != null &&
            GameManager.instance.ui != null)
        {
            GameManager.instance.ui.UpdateCountdown(seconds);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void ShowMatchFoundClientRpc()
    {
        if (GameManager.instance != null &&
            GameManager.instance.ui != null)
        {
            GameManager.instance.ui.ShowMatchFound();
        }
    }

    private bool HasInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    private bool CheckInternet()
    {
        if (HasInternet())
            return true;

        GameManager.instance.ui.OpenOfflinePanel();

        return false;
    }
}
