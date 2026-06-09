using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log(
            $"PLAYER SPAWNED | ClientId: {OwnerClientId} | IsOwner: {IsOwner}"
        );

        if (IsOwner)
        {
            GameManager.instance.networkPlayer =
                GetComponent<player>();

            Debug.Log("LOCAL PLAYER ASSIGNED TO GAMEMANAGER");
        }

        if (IsServer)
        {
            Transform[] spawnPoints =
                GameObject.Find("SpawnPoints")
                .GetComponentsInChildren<Transform>();

            int spawnIndex = (int)OwnerClientId + 1;

            if (spawnIndex < spawnPoints.Length)
            {
                transform.position =
                    spawnPoints[spawnIndex].position;
            }

            Debug.Log("SERVER SEES PLAYER");
        }

        if (IsClient)
        {
            Debug.Log("CLIENT SEES PLAYER");
        }
    }
}