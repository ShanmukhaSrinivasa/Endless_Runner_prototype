using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    private int amountOfCoins;
    [SerializeField] private GameObject CoinPrefab;

    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;

    [SerializeField] private SpriteRenderer[] coinImg;
    [SerializeField] private int generatorId;


    void Start()
    {
        //for (int i = 1; i < coinImg.Length; i++)
        //{
        //    coinImg[i].sprite = null;
        //}


        if (GameManager.instance.IsMultiPlayer())
        {
            if (MultiplayerMatchManager.Instance == null)
            {
                Debug.LogWarning("MultiplayerMatchManager not found!");

                return;
            }

            int seed = MultiplayerMatchManager.Instance.GetWorldSeed();

            int deterministicValue = Mathf.Abs(seed +generatorId * 3571 +Mathf.RoundToInt(transform.position.x));

            amountOfCoins = minCoins +(deterministicValue %(maxCoins - minCoins + 1));
        }
        else
        {
            amountOfCoins = Random.Range(minCoins,maxCoins + 1);
        }

        int additionalOffset = amountOfCoins / 2;

        for (int i = 0; i < amountOfCoins; i++)
        {
            Vector3 offSet = new Vector2(i - additionalOffset, 0);
            Instantiate(CoinPrefab, transform.position + offSet, Quaternion.identity, transform);
        }
    }

}
