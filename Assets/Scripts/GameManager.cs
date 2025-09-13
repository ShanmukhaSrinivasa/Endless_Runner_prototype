using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public player player;

    public bool colorEntirePlatform;

    [Header("Color Info")]
    public Color platformColor;
    public Color playerColor;


    [Header("Score Info")]
    public int coins;
    public float distance;

    private void Awake()
    {
        instance = this;
    }

    public void RestartLevel()
    {
        SaveInfo();
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
        int savedCoins = PlayerPrefs.GetInt("Coins");

        PlayerPrefs.SetInt("Coins", savedCoins + coins);

        float score = distance * coins;

        PlayerPrefs.SetFloat("LastScore", score);

        if (PlayerPrefs.GetFloat("HighScore") < score)
        {
            PlayerPrefs.SetFloat("HighScore", score);
        }
    }
}
