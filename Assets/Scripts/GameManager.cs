using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UI_Main ui;

    public player player;

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

    private void Awake()
    {
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
        if (player != null)
        {
            player.GetComponent<SpriteRenderer>().color = playerColor;
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

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (player.transform.position.x > distance)
        {
            distance = player.transform.position.x;
        }
    }

    public void UnlockPlayer() => player.playerUnlocked = true;

    public void SaveInfo()
    {
        int myCoins = PlayerPrefs.GetInt("Coins");

        PlayerPrefs.SetInt("Coins", myCoins + coins);

        score = distance * coins;

        PlayerPrefs.SetFloat("LastScore", score);

        if (PlayerPrefs.GetFloat("HighScore") < score)
        {
            PlayerPrefs.SetFloat("HighScore", score);
        }
    }

    public void GameEnded()
    {
        SaveInfo();
        ui.OpenEndGameUI();
    }
}
