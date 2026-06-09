using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    private bool gamePaused;
    private bool gameMuted;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject endGame;
    [SerializeField] private GameObject multiplayerPanel;
    [SerializeField] private GameObject inGameUI;
    [Space]

    [Header("VFX")]
    [SerializeField] private ParticleSystem[] fireWorks;
    [Space]

    [SerializeField] private TextMeshProUGUI lastScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Volume info")]
    [SerializeField] private UI_SliderVolume[] sliders;
    [SerializeField] private Image muteIcon;
    [SerializeField] private Image inGameMuteIcon;

    [Header("Multiplayer Lobby info")]
    [SerializeField] private GameObject multiplayerLobbyPanel;

    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI lobbyRoomCodeText;

    [SerializeField] private Button startMatchButton;

    private void Start()
    {
        SwitchMenuTo(mainMenu);

        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].SetupSlider();
        }

        lastScoreText.text = "Last Score:  " + PlayerPrefs.GetFloat("LastScore").ToString("#,#");
        highScoreText.text = "High Score:  " + PlayerPrefs.GetFloat("HighScore").ToString("#,#");

    }

    public void SwitchMenuTo(GameObject uiMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        uiMenu.SetActive(true);

        AudioManager.instance.PlaySFX(3);
        coinsText.text = PlayerPrefs.GetInt("Coins").ToString("#,#");
    }

    public void GameMutedButton()
    {
        gameMuted = !gameMuted; //Works like a switch

        if (gameMuted)
        {
            muteIcon.color = new Color(1, 1, 1, .3f);
            AudioListener.volume = 0;
        }
        else
        {
            muteIcon.color = Color.white;
            AudioListener.volume = 1;
        }
    }
    public void StartGameButton()
    {
        muteIcon = inGameMuteIcon;

        for(int i=0; i<fireWorks.Length; i++)
        {
            fireWorks[i].Play();
        }
        
        if (gameMuted)
        {
            muteIcon.color = new Color(1, 1, 1, .3f);
        }
        GameManager.instance.BeginGamePlay();
    }

    public void GamePauseButton()
    {
        if (gamePaused)
        {
            Time.timeScale = 1;
            gamePaused = false;
        }
        else
        {
            Time.timeScale = 0;
            gamePaused = true;
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void RestartGameButton() => GameManager.instance.RestartLevel(); 

    public void OpenEndGameUI()
    {
        SwitchMenuTo(endGame);
    }

    public void StartSinglePlayer()
    {
        GameManager.instance.currentGameMode = GameMode.SinglePlayer;

        Debug.Log("Start Single Player");

        StartGameButton();
    }

    public void StartMultiPlayer()
    {
        GameManager.instance.currentGameMode = GameMode.Multiplayer;

        Debug.Log("Start MultiPlayer");

        GameManager.instance.StartGame();

        // Temporary
        SwitchMenuTo(mainMenu);
    }

    public void OpenMultiplayerPanel()
    {
        mainMenu.SetActive(false);
        multiplayerPanel.SetActive(true);

        Debug.Log("Multiplayer Panel opened");
    }

    public void CloseMutiplayerPanel()
    {
        multiplayerPanel.SetActive(false);
        mainMenu.SetActive(true);

        Debug.Log("Returned to main menu");
    }

    public void CreateRoom()
    {
        Debug.Log("Create room clicked");
    }

    public void JoinRoom()
    {
        Debug.Log("Join room clicked");
    }

    public void OpenMultiplayerLobby()
    {
        multiplayerLobbyPanel.SetActive(true);

        playerCountText.text = "Players: 1/2";
        statusText.text = "Waiting for players...";

        bool isHost = false;

        if (NetworkManager.Singleton != null)
        {
            isHost = NetworkManager.Singleton.IsHost;
        }

        startMatchButton.gameObject.SetActive(isHost);
        startMatchButton.interactable = false;

        Debug.Log("OPEN LOBBY | IsHost = " + isHost);
    }

    public void UpdatePlayerCount(int count)
    {
        playerCountText.text = $"Players: {count}/2";

        if (count >= 2)
        {
            statusText.text = "Ready To Start";

            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.IsHost)
            {
                startMatchButton.interactable = true;
            }
        }
        else
        {
            statusText.text = "Waiting for players...";

            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.IsHost)
            {
                startMatchButton.interactable = false;
            }
        }
    }

    public void SetLobbyRoomCode(string roomCode)
    {
        lobbyRoomCodeText.text = $"Room Code: {roomCode}";
    }

    public void StartMatchButton()
    {
        MultiplayerLobbyManager.Instance.StartMatchRpc();
    }

    public void CloseMultiplayerLobby()
    {
        multiplayerLobbyPanel.SetActive(false);
    }

    public void HideAllMenus()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OpenInGameUI()
    {
        HideAllMenus();

        if (inGameUI != null)
        {
            inGameUI.SetActive(true);
        }
    }
}
