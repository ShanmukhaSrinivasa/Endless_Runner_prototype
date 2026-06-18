using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_Main : MonoBehaviour
{
    private bool gamePaused;
    private bool gameMuted;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject endGame;
    [SerializeField] private GameObject multiplayerPanel;
    [SerializeField] private GameObject endGameMultiplayer;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject loginUI;
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

    [SerializeField] private TextMeshProUGUI hostNameText;
    [SerializeField] private TextMeshProUGUI guestNameText;

    [SerializeField] private Button startMatchButton;

    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI winnerNameText;
    [SerializeField] private TextMeshProUGUI multiplayerDistanceText;
    [SerializeField] private TextMeshProUGUI resultTitleText;

    [SerializeField] private TextMeshProUGUI hostReadyText;
    [SerializeField] private TextMeshProUGUI guestReadyText;

    [Header("Multiplayer Stats info")]
    [SerializeField] private TextMeshProUGUI profileUserNameText;
    [SerializeField] private TextMeshProUGUI winsText;
    [SerializeField] private TextMeshProUGUI lossesText;
    [SerializeField] private TextMeshProUGUI bestDistanceText;

    [SerializeField] private GameObject searchingPanel;
    [SerializeField] private GameObject roomCodeContainer;
    [SerializeField] private GameObject readyContainer;
    [SerializeField] private TextMeshProUGUI matchmakingText;
    [SerializeField] private TextMeshProUGUI lobbyTitleText;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TMP_Text searchingText;

    [SerializeField] private TextMeshProUGUI countdownText;

    [SerializeField] private GameObject offlinePanel;

    [Header("Multiplayer Disconnect info")]
    [SerializeField] private GameObject disconnectPanel;
    [SerializeField] private TMP_Text disconnectText;

    [SerializeField] private GameObject quickMatchTimeoutPanel;
    [SerializeField] private TMP_Text waitingTimerText;

    private void Start()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].SetupSlider();
        }

        lastScoreText.text = "Last Score:  " + PlayerPrefs.GetFloat("LastScore").ToString("#,#");
        highScoreText.text = "High Score:  " + PlayerPrefs.GetFloat("HighScore").ToString("#,#");

        UpdateProfile();
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

    public void RestartGameButton()
    {
        GameManager.instance.FinalizeRun();

        GameManager.instance.RestartLevel();
    }

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

        if (MultiplayerLobbyManager.Instance != null &&
            MultiplayerLobbyManager.Instance.IsQuickMatch)
        {
            statusText.text = "Searching For Opponent...";
        }
        else
        {
            statusText.text = "Waiting for players...";
        }

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
            if (MultiplayerLobbyManager.Instance != null && MultiplayerLobbyManager.Instance.IsQuickMatch)
            {
                statusText.text = "Opponent Found!";
                matchmakingText.text = "Opponent Found!";
            }
            else
            {
                statusText.text = "Waiting For Ready...";
            }
        }
        else
        {
            statusText.text = "Waiting for players...";

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
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

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(GameManager.instance.IsSinglePlayer());
        }
    }

    public void UpdatePlayerNames(string hostName,string guestName)
    {
        hostNameText.text = "Host: " + hostName;

        if (string.IsNullOrEmpty(guestName))
        {
            guestNameText.text = "Guest: Waiting...";
        }
        else
        {
            guestNameText.text = "Guest: " + guestName;
        }
    }

    public void OpenMultiplayerResultUI(bool isWinner,string winnerName,float distance)
    {
        SwitchMenuTo(endGameMultiplayer);

        if (isWinner)
        {
            resultTitleText.text = "VICTORY";
            resultText.text = "WINNER";
            winnerNameText.text = "Reward: +100 Coins";
        }
        else
        {
            resultTitleText.text = "GAME OVER";
            resultText.text = "ELIMINATED";
            winnerNameText.text ="Winner: " + winnerName + "\nYou get +25 Coins";
        }

        multiplayerDistanceText.text ="Distance: " +distance.ToString("#,#") +" m";
    }
    public void ReturnToMainMenu()
    {
        GameManager.instance.FinalizeRun();

        Time.timeScale = 1f;

        SceneManager.LoadScene("Endless_Runner");
    }

    public void UpdateReadyStatus(bool hostReady,bool guestReady)
    {
        if (MultiplayerLobbyManager.Instance != null && MultiplayerLobbyManager.Instance.IsQuickMatch)
        {
            return;
        }

        hostReadyText.text =hostReady? "Host Ready: YES": "Host Ready: NO";

        guestReadyText.text =guestReady? "Guest Ready: YES": "Guest Ready: NO";

        bool canStart =hostReady &&guestReady;

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            startMatchButton.interactable = canStart;
        }
    }

    public void UpdateProfile()
    {
        profileUserNameText.text = LoginManager.Instance.GetPlayerName();

        winsText.text = "Wins: " + PlayerStats.Wins;

        lossesText.text = "Losses: " + PlayerStats.Losses;

        bestDistanceText.text = "Best: " + Mathf.RoundToInt(PlayerStats.BestDistance) + "m";
    }

    public void OpenSearchingPanel(string message = "Searching For Opponent...")
    {
        if (searchingPanel != null)
        {
            searchingPanel.SetActive(true);
        }

        if (searchingText != null)
        {
            searchingText.text = message;
        }
    }

    public void CloseSearchingPanel()
    {
        searchingPanel.SetActive(false);
    }

    public void ConfigureLobbyUI(bool isQuickMatch)
    {
        roomCodeContainer.SetActive(!isQuickMatch);
        readyContainer.SetActive(!isQuickMatch);

        if (readyButton != null)
        {
            readyButton.SetActive(!isQuickMatch);
        }

        if (isQuickMatch)
        {
            matchmakingText.gameObject.SetActive(true);

            matchmakingText.text = "Searching For Opponent...";

            lobbyTitleText.text = "MATCHMAKING";
        }
        else
        {
            matchmakingText.gameObject.SetActive(false);

            lobbyTitleText.text = "FRIEND ROOM";
        }
    }

    public void ShowMatchFound()
    {
        matchmakingText.text = "Opponent Found!";
    }
    public void UpdateCountdown(int seconds)
    {
        matchmakingText.text = "Starting In " + seconds;
    }

    public void RefreshHostUI()
    {
        bool isHost = NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost;

        startMatchButton.gameObject.SetActive(isHost);
    }

    public IEnumerator ReviveCountdown()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(.5f);

        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenOfflinePanel()
    {
        if (offlinePanel != null)
        {
            offlinePanel.SetActive(true);
        }
    }

    public void CloseOfflinePanel()
    {
        if (offlinePanel != null)
        {
            offlinePanel.SetActive(false);
        }
    }

    public void RetryInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }

        CloseOfflinePanel();
    }

    public void ShowDisconnectMessage(string message)
    {
        disconnectPanel.SetActive(true);

        if (disconnectText != null)
        {
            disconnectText.text = message;
        }
    }

    public void UpdateWaitingTimer(int seconds)
    {
        if (waitingTimerText != null)
        {
            waitingTimerText.text = $"{seconds}";
        }
    }

    public void ShowQuickMatchTimeout()
    {
        quickMatchTimeoutPanel.SetActive(true);
    }

    public void HideQuickMatchTimeout()
    {
        quickMatchTimeoutPanel.SetActive(false);
    }
}
