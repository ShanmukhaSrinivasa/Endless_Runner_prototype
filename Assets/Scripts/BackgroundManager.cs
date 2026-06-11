using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private GameObject[] backgrounds;

    private void Start()
    {
        SelectBackground();
    }

    private void SelectBackground()
    {
        foreach (GameObject bg in backgrounds)
        {
            bg.SetActive(false);
        }

        int backgroundIndex;

        if (GameManager.instance.IsMultiPlayer())
        {
            int seed =MultiplayerMatchManager.Instance.GetWorldSeed();

            backgroundIndex =Mathf.Abs(seed) %backgrounds.Length;
        }
        else
        {
            backgroundIndex = Random.Range(0,backgrounds.Length);
        }

        backgrounds[backgroundIndex].SetActive(true);
    }
}