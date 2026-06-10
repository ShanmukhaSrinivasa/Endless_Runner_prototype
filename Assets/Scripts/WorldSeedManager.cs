using Unity.Netcode;
using UnityEngine;

public class WorldSeedManager : NetworkBehaviour
{
    public static WorldSeedManager Instance;

    public NetworkVariable<int> WorldSeed =new NetworkVariable<int>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            WorldSeed.Value = Random.Range(0, 999999);
        }
    }
}