using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[Serializable]
public struct ColorToSell
{
    public Color color;
    public int price;
}

public enum ColorType
{
    playerColor,
    platformColor
}

public class UI_Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI notifyText;
    [Space]

    [Header("Platform Color Info")]
    [SerializeField] private GameObject platformColorButton;
    [SerializeField] private Transform platformColorParent;
    [SerializeField] private Image platformDisplay;
    [SerializeField] private ColorToSell[] platformColor;

    [Header("Player Color Info")]
    [SerializeField] private GameObject playerColorButton;
    [SerializeField] private Transform playerColorParent;
    [SerializeField] private Image playerDisplay;
    [SerializeField] private ColorToSell[] playerColor;

    void Start()
    {
        coinsText.text = PlayerPrefs.GetInt("Coins").ToString("#,#");

        for (int i=0; i<platformColor.Length; i++)
        {
            Color color = platformColor[i].color;
            int price = platformColor[i].price;

            GameObject newButton = Instantiate(platformColorButton, platformColorParent);

            newButton.transform.GetChild(0).GetComponent<Image>().color = color;
            newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text= price.ToString();

            newButton.GetComponent<Button>().onClick.AddListener(() => PlatformPurchaseColor(color, price, ColorType.platformColor));
        }

        for (int i = 0; i < playerColor.Length; i++)
        {
            Color color = playerColor[i].color;
            int price = playerColor[i].price;

            GameObject newButton = Instantiate(playerColorButton, playerColorParent);

            newButton.transform.GetChild(0).GetComponent<Image>().color = color;
            newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString();

            newButton.GetComponent<Button>().onClick.AddListener(() => PlatformPurchaseColor(color, price, ColorType.playerColor));
        }

    }

    public void PlatformPurchaseColor(Color color, int price, ColorType colorType)
    {
        if (EnoughMoney(price))
        {
            if (colorType == ColorType.platformColor)
            {
                GameManager.instance.platformColor = color;
                GameManager.instance.SaveColor(color.r, color.g, color.b, color.a);
                platformDisplay.color = color;
            }
            else if (colorType == ColorType.playerColor)
            {
                GameManager.instance.player.GetComponent<SpriteRenderer>().color = color;
                GameManager.instance.SaveColor(color.r, color.g, color.b, color.a);
                playerDisplay.color = color;
            }

            StartCoroutine(NotifyText("Purchase Succesfull", 2));
        }
        else
        {
            StartCoroutine(NotifyText("Not enough money!", 2));
        }
    }

    private bool EnoughMoney(int price)
    {
        int myCoins = PlayerPrefs.GetInt("Coins");

        if (myCoins > price)
        {
            int newAmountOfCoins = myCoins - price;
            PlayerPrefs.SetInt("Coins", newAmountOfCoins);
            coinsText.text = PlayerPrefs.GetInt("Coins").ToString("#,#");
            Debug.Log("Purchase Succesfull");
            return true;
        }

        Debug.Log("Not enough money");
        return false;
    }

    IEnumerator NotifyText(string text, float seconds)
    {
        notifyText.text = text;

        yield return new WaitForSeconds(seconds);

        notifyText.text = "SHOP";
    }
}
