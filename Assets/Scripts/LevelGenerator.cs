using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] levelPart;
    [SerializeField] private Vector3 nextPosition;

    [SerializeField] private float distanceToSpawn;
    [SerializeField] private float distanceToDelete;
    [SerializeField] private Transform player;

    // Update is called once per frame
    void Update()
    {
        deletePlatfrom();
        GeneratePlatform();

    }

    private void GeneratePlatform()
    {
        while (Vector2.Distance(player.transform.position, nextPosition) < distanceToSpawn)
        {
            Transform part = levelPart[Random.Range(0, levelPart.Length)];

            Vector2 newPosition = new Vector2(nextPosition.x - part.Find("StartPoint").localPosition.x, 0);

            Transform newPart = Instantiate(part, newPosition, transform.rotation, transform);

            nextPosition = newPart.Find("EndPoint").position;
        }
    }

    private void deletePlatfrom()
    {
        if(transform.childCount > 0)
        {
            Transform partToDelete = transform.GetChild(0);

            if(Vector2.Distance(player.transform.position, partToDelete.transform.position) > distanceToDelete)
            {
                Destroy(partToDelete.gameObject);
            }
        }
    }
}
