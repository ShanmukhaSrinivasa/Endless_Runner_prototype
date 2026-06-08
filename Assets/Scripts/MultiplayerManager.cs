using Unity.VisualScripting;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance;

    public bool IsInRoom { get; private set;}

    public string CurrentRoomCode { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CreateRoom()
    {
        IsInRoom = true;
        CurrentRoomCode = "TEST:01";

        Debug.Log($"Created Room: {CurrentRoomCode}");
    }

    public void JoinRoom()
    {
        IsInRoom = true;
        CurrentRoomCode = "TEST:01";

        Debug.Log($"Joined Room: {CurrentRoomCode}");
    }
}
