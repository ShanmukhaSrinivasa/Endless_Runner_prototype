using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ColorToSell
{
    public Color color;
    public int price;
}

public class UI_Shop : MonoBehaviour
{
    [SerializeField] private GameObject platformColorButton;
    [SerializeField] private Transform platformColorParent;

    [SerializeField] private ColorToSell[] platformColor;



    void Start()
    {
        for(int i=0; i<platformColor.Length; i++)
        {
            GameObject newButton = Instantiate(platformColorButton, platformColorParent);

            newButton.transform.GetChild(0).GetComponent<Image>().color = platformColor[i].color;
            newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text= platformColor[i].price.ToString();

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
