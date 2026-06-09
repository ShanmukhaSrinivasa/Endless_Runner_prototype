using UnityEngine;

public class Trigger_DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<player>() != null || collision.GetComponent<PlayerSinglePlayer>() != null)
        {
            GameManager.instance.GameEnded();
        }
    }
}
