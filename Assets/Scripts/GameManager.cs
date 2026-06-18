using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameMode currentGameMode = GameMode.SinglePlayer;

    [SerializeField] private PlayerSinglePlayer singlePlayerPrefab;
    [SerializeField] private Transform singlePlayerSpawnPoint;

    public UI_Main ui;

    public player networkPlayer;
    public PlayerSinglePlayer singlePlayerPlayer;

    public bool colorEntirePlatform;

    [Header("Color Info")]
    public Color defaultPlatformColor = Color.green;
    public Color defaultPlayerColor = Color.white;
    public Color platformColor;
    public Color playerColor;


    [Header("Score Info")]
    public int coins;
    public float distance;
    public float score;

    public bool gamePlayStarted = false;
    private bool isGameOver = false;
    private bool hasRevived = false;
    private bool saveCompleted = false;
    private bool runSaved = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Time.timeScale = 1;

        // On the very first launch, set the default colors as "owned"
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            PlayerPrefs.SetInt("FirstLaunch", 1);
            PlayerPrefs.SetInt("platformColor_Owned_0", 1); // Mark default platform color as owned
            PlayerPrefs.SetInt("playerColor_Owned_0", 1);   // Mark default player color as owned
            PlayerPrefs.Save();
        }

        LoadPlatformColor();
        LoadPlayerColor();
    }

    private void Start()
    {
        if (singlePlayerPlayer != null)
        {
            singlePlayerPlayer.GetComponent<SpriteRenderer>().color = playerColor;
        }
    }

    // --- SEPARATE SAVE/LOAD FOR PLATFORM COLOR ---
    public void SavePlatformColor(Color colorToSave)
    {
        PlayerPrefs.SetFloat("PlatformColorR", colorToSave.r);
        PlayerPrefs.SetFloat("PlatformColorG", colorToSave.g);
        PlayerPrefs.SetFloat("PlatformColorB", colorToSave.b);
        PlayerPrefs.SetFloat("PlatformColorA", colorToSave.a);
    }

    public void LoadPlatformColor()
    {
        if (PlayerPrefs.HasKey("PlatformColorR"))
        {
            platformColor = new Color(
                PlayerPrefs.GetFloat("PlatformColorR"),
                PlayerPrefs.GetFloat("PlatformColorG"),
                PlayerPrefs.GetFloat("PlatformColorB"),
                PlayerPrefs.GetFloat("PlatformColorA")
            );
        }
        else
        {
            platformColor = defaultPlatformColor;
        }
    }

    // --- SEPARATE SAVE/LOAD FOR PLAYER COLOR ---
    public void SavePlayerColor(Color colorToSave)
    {
        PlayerPrefs.SetFloat("PlayerColorR", colorToSave.r);
        PlayerPrefs.SetFloat("PlayerColorG", colorToSave.g);
        PlayerPrefs.SetFloat("PlayerColorB", colorToSave.b);
        PlayerPrefs.SetFloat("PlayerColorA", colorToSave.a);
    }

    public void LoadPlayerColor()
    {
        if (PlayerPrefs.HasKey("PlayerColorR"))
        {
            playerColor = new Color(
                PlayerPrefs.GetFloat("PlayerColorR"),
                PlayerPrefs.GetFloat("PlayerColorG"),
                PlayerPrefs.GetFloat("PlayerColorB"),
                PlayerPrefs.GetFloat("PlayerColorA")
            );
        }
        else
        {
            playerColor = defaultPlayerColor;
        }

    }

    public void BeginGamePlay()
    {
        gamePlayStarted = true;

        if (IsSinglePlayer())
        {
            StartSinglePlayer();
        }
        else
        {
            StartMultiPlayer();
        }

        UnlockPlayer();
    }

    public void StartGame()
    {
        if (currentGameMode == GameMode.SinglePlayer)
        {
            StartSinglePlayer();
        }
        else if (currentGameMode == GameMode.Multiplayer)
        {
            StartMultiPlayer();
        }
    }

    private void StartSinglePlayer()
    {
        if (singlePlayerPlayer == null)
        {
            singlePlayerPlayer = Instantiate(singlePlayerPrefab,singlePlayerSpawnPoint.position,Quaternion.identity);

            singlePlayerPlayer.GetComponent<SpriteRenderer>().color = playerColor;
        }

        Debug.Log("Single Player Systems Started");

        CinemachineCamera cineCam =FindFirstObjectByType<CinemachineCamera>();

        if (cineCam != null)
        {
            cineCam.Target.TrackingTarget = singlePlayerPlayer.transform;
        }
    }

    private void StartMultiPlayer()
    {
        Debug.Log("Multiplayer Systems Started");

        CinemachineCamera cineCam =FindFirstObjectByType<CinemachineCamera>();

        if (cineCam != null && networkPlayer != null)
        {
            cineCam.Target.TrackingTarget =networkPlayer.transform;

            Debug.Log("CAMERA FOLLOWING NETWORK PLAYER");
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Endless_Runner");
    }

    private void Update()
    {
        if (IsSinglePlayer())
        {
            if (singlePlayerPlayer == null)
                return;

            if (singlePlayerPlayer.transform.position.x > distance)
            {
                distance = singlePlayerPlayer.transform.position.x;
            }
        }
        else
        {
            if (networkPlayer == null)
                return;

            if (networkPlayer.transform.position.x > distance)
            {
                distance = networkPlayer.transform.position.x;
            }
        }
    }

    public void UnlockPlayer()
    {
        Debug.Log("UNLOCK PLAYER CALLED");
        Debug.Log("MODE = " + currentGameMode);

        if (IsSinglePlayer())
        {
            Debug.Log("SINGLE PLAYER BRANCH");
        }
        else
        {
            Debug.Log("MULTIPLAYER BRANCH");

            if (networkPlayer != null)
            {
                //Debug.Log("PLAYER BEFORE UNLOCK = " + networkPlayer.playerUnlocked);

                networkPlayer.playerUnlocked = true;

                //Debug.Log("PLAYER AFTER UNLOCK = " + networkPlayer.playerUnlocked);
                Debug.Log("NETWORK PLAYER UNLOCKED");
            }
            else
            {
                Debug.Log("NETWORK PLAYER IS NULL");
            }
        }
    }

    public void SaveInfo()
    {
        if (runSaved)
        {
            return;
        }

        runSaved = true;

        int myCoins = PlayerPrefs.GetInt("Coins");

        PlayerPrefs.SetInt("Coins", myCoins + coins);

        score = distance * coins;

        PlayerPrefs.SetFloat("LastScore", score);

        if (PlayerPrefs.GetFloat("HighScore") < score)
        {
            PlayerPrefs.SetFloat("HighScore", score);
        }

        PlayerPrefs.Save();
    }

    public void GameEnded()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        Time.timeScale = 0f;

        if (IsSinglePlayer())
        {
            ui.OpenEndGameUI();
        }
    }

    public bool IsSinglePlayer()
    {
        return currentGameMode == GameMode.SinglePlayer;
    }

    public bool IsMultiPlayer()
    {
        return currentGameMode == GameMode.Multiplayer;
    }

    public bool IsGameplayStarted()
    {
        return gamePlayStarted;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsCurrentPlayerColor(Color color)
    {
        return playerColor == color;
    }

    public bool IsCurrentPlatformColor(Color color)
    {
        return platformColor == color;
    }

    public int GetCurrentColorPrice()
    {
        return PlayerPrefs.GetInt("CurrentColorPrice",200);
    }

    public void IncreaseColorPrice()
    {
        int currentPrice = GetCurrentColorPrice();

        currentPrice =Mathf.RoundToInt(currentPrice * 1.5f);

        PlayerPrefs.SetInt("CurrentColorPrice",currentPrice);

        PlayerPrefs.Save();
    }

    public bool HasRevived()
    {
        return hasRevived;
    }

    public void MarkRevived()
    {
        hasRevived = true;
    }

    public void RevivePlayer()
    {
        if (hasRevived)
        {
            return;
        }

        hasRevived = true;

        isGameOver = false;

        ui.OpenInGameUI();

        if (singlePlayerPlayer != null)
        {
            singlePlayerPlayer.Revive();
        }

        StartCoroutine(ui.ReviveCountdown());
    }

    public void FinalizeRun()
    {
        if (saveCompleted)
            return;

        saveCompleted = true;

        SaveInfo();
    }
}
