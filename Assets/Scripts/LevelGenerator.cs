using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] levelPart;
    [SerializeField] private Vector3 nextPosition;

    [SerializeField] private float distanceToSpawn = 50f;
    [SerializeField] private float distanceToDelete = 200f;

    [SerializeField] private Transform player;

    private void Update()
    {
        // Always keep the correct player reference
        if (GameManager.instance.IsSinglePlayer())
        {
            if (GameManager.instance.singlePlayerPlayer != null)
            {
                player = GameManager.instance.singlePlayerPlayer.transform;
            }
        }
        else
        {
            if (GameManager.instance.networkPlayer != null)
            {
                player = GameManager.instance.networkPlayer.transform;
            }
        }

        if (player == null)
            return;

        if (!GameManager.instance.IsGameplayStarted())
            return;

        GeneratePlatform();
        DeletePlatform();
    }

    private void GeneratePlatform()
    {
        float xDistance = nextPosition.x - player.position.x;

        if (xDistance < distanceToSpawn)
        {
            Transform part =levelPart[Random.Range(0, levelPart.Length)];

            Transform startPoint =part.Find("StartPoint");

            if (startPoint == null)
            {
                Debug.LogError(part.name + " MISSING STARTPOINT!");
                return;
            }

            Vector2 spawnPosition =new Vector2(nextPosition.x -startPoint.localPosition.x,0f);

            Transform newPart =Instantiate(part,spawnPosition,Quaternion.identity,transform);

            Transform endPoint =newPart.Find("EndPoint");

            if (endPoint == null)
            {
                Debug.LogError(part.name + " MISSING ENDPOINT!");
                return;
            }

            nextPosition = endPoint.position;
        }
    }

    private void DeletePlatform()
    {
        Transform partToDelete = null;

        foreach (Transform part in transform)
        {
            float distanceBehindPlayer =player.position.x - part.position.x;

            if (distanceBehindPlayer > distanceToDelete)
            {
                partToDelete = part;
                break;
            }
        }

        if (partToDelete != null)
        {
            Destroy(partToDelete.gameObject);
        }
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}