using System.Runtime.CompilerServices;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    private int amountOfCoins;
    [SerializeField] private GameObject CoinPrefab;

    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;
    void Start()
    {
        amountOfCoins = Random.Range(minCoins, maxCoins);
        int additionalOffset = amountOfCoins / 2;

        for (int i = 0; i < amountOfCoins; i++)
        {
            Vector3 offSet = new Vector2(i - additionalOffset, 0);
            Instantiate(CoinPrefab, transform.position + offSet, Quaternion.identity, transform);
        }
    }

}
