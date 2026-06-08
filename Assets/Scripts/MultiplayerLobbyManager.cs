using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerLobbyManager : MonoBehaviour
{
    public static MultiplayerLobbyManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TMP_InputField roomCodeInput;

    [Header("Relay")]
    private string relayJoinCode;


    private Lobby currentLobby;

    private void Awake()
    {
        Instance = this;
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
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("PixelDashRoom", 5);


            Debug.Log("Lobby created!");
            Debug.Log("Lobby Code: " +  currentLobby.LobbyCode);

            if (roomCodeText != null)
            {
                roomCodeText.text = "Room Code: " + currentLobby.LobbyCode;
            }
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
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Relay Created!");
            Debug.Log("Relay Join Code: " + relayJoinCode);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public string GetLobbyCode()
    {
        if (currentLobby == null)
            return "";

        return currentLobby.LobbyCode;
    }
}
