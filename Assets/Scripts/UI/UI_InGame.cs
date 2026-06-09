using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private PlayerSinglePlayer singlePlayer;
    private player networkPlayer;

    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI distanceText;

    [SerializeField] private Image heartEmpty;
    [SerializeField] private Image heartFull;
    [SerializeField] private Image slideIcon;


    private int coins;
    private float distance;


    private void Start()
    {
        if (GameManager.instance.IsSinglePlayer())
        {
            singlePlayer = GameManager.instance.singlePlayerPlayer;
        }
        else
        {
            networkPlayer = GameManager.instance.networkPlayer;
        }

        InvokeRepeating("UpdateInfo", 0, .2f);
    }

    private void UpdateInfo()
    {
        if (GameManager.instance.IsSinglePlayer())
        {
            slideIcon.enabled = singlePlayer.slideCooldownCounter < 0;
            heartEmpty.enabled = !singlePlayer.extraLife;
            heartFull.enabled = singlePlayer.extraLife;
        }
        else
        {
            slideIcon.enabled = networkPlayer.slideCooldownCounter < 0;
            heartEmpty.enabled = !networkPlayer.extraLife;
            heartFull.enabled = networkPlayer.extraLife;
        }

        distance = GameManager.instance.distance;
        coins = GameManager.instance.coins;

        if (distance > 0)
        {
            distanceText.text = distance.ToString("#,#") + "  m";
        }

        if (coins > 0)
        {
            coinsText.text = GameManager.instance.coins.ToString("#,#");
        }
    }
}
