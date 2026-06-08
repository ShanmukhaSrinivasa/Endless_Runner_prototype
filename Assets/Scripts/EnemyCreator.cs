using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform respawnPosition;
    [SerializeField] private float chanceToRespawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<player>() != null)
        {
            if (Random.Range(0, 100) < chanceToRespawn)
            {
                GameObject newEenemy = Instantiate(enemyPrefab, respawnPosition.position, Quaternion.identity);
                Destroy(newEenemy, 30);
            }
        }
    }
}
