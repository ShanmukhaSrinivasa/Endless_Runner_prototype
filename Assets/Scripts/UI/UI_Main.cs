using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    private bool gamePaused;
    private bool gameMuted;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject endGame;
    [Space]

    [SerializeField] private TextMeshProUGUI lastScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Volume info")]
    [SerializeField] private UI_SliderVolume[] sliders;
    [SerializeField] private Image muteIcon;
    [SerializeField] private Image inGameMuteIcon;


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
    public void startGameButton()
    {
        muteIcon = inGameMuteIcon;
        
        if (gameMuted)
        {
            muteIcon.color = new Color(1, 1, 1, .3f);
        }
        GameManager.instance.UnlockPlayer();
    }

    public void gamePauseButton()
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

    public void RestartGameButton() => GameManager.instance.RestartLevel(); 

    public void OpenEndGameUI()
    {
        SwitchMenuTo(endGame);
    }
}
