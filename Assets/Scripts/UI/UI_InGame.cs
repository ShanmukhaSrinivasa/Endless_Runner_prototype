using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private player player;

    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI distanceText;

    [SerializeField] private Image heartEmpty;
    [SerializeField] private Image heartFull;

    private int coins;
    private float distance;


    private void Start()
    {
        player = GameManager.instance.player;
        InvokeRepeating("updateInfo", 0, .2f);
        
    }

    private void updateInfo()
    {
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

        heartEmpty.enabled = !player.extraLife;
        heartFull.enabled = !player.extraLife;
    }
}
