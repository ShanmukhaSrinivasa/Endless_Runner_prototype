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
        float distance = Vector2.Distance(
            player.position,
            nextPosition
        );

        if (distance < distanceToSpawn)
        {
            Transform part =
                levelPart[Random.Range(0, levelPart.Length)];

            Vector2 spawnPosition =
                new Vector2(
                    nextPosition.x -
                    part.Find("StartPoint").localPosition.x,
                    0f
                );

            Transform newPart =
                Instantiate(
                    part,
                    spawnPosition,
                    Quaternion.identity,
                    transform
                );

            Transform endPoint =
                newPart.Find("EndPoint");

            nextPosition = endPoint.position;
        }
    }

    private void DeletePlatform()
    {
        if (transform.childCount <= 0)
            return;

        Transform partToDelete =
            transform.GetChild(0);

        float distance =
            Vector2.Distance(
                player.position,
                partToDelete.position
            );

        if (distance > distanceToDelete)
        {
            Destroy(partToDelete.gameObject);
        }
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}