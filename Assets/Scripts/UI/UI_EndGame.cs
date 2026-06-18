using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_EndGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distance;
    [SerializeField] private TextMeshProUGUI coins;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject reviveButton;

    private void OnEnable()
    {
        GameManager manager = GameManager.instance;

        if (reviveButton != null)
        {
            reviveButton.SetActive(!GameManager.instance.HasRevived());
        }

        distance.text = "Distance: " + manager.distance.ToString("#,#") + "  m";
        coins.text = "Coins: " + manager.coins.ToString("#,#");

        float currentScore = manager.distance * manager.coins;
        score.text = "Score: " + currentScore.ToString("#,#");
    }

    public void ReviveButton()
    {
        GameManager.instance.RevivePlayer();
    }
}
