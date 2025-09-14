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
        LoadColor();
    }

    public void SaveColor(float rP, float gP, float bP, float aP)
    {
        PlayerPrefs.SetFloat("PlatformColorR", rP);
        PlayerPrefs.SetFloat("PlatformColorG", gP);
        PlayerPrefs.SetFloat("PlatformColorB", bP);
        PlayerPrefs.SetFloat("PlatformColorA", aP);
    }

    public void LoadColor()
    {
        if(PlayerPrefs.HasKey("PlatformColorR"))
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

            Color newColor = new Color(PlayerPrefs.GetFloat("PlatformColorR"),
                                               PlayerPrefs.GetFloat("PlatformColorG"),
                                               PlayerPrefs.GetFloat("PlatformColorB"),
                                               PlayerPrefs.GetFloat("PlatformColorA", 1));

            platformColor = newColor;
            sr.color = newColor;
        }
        else
        {
            platformColor = defaultPlatformColor;
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
