using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private List<Transform> _spawnPoints;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            PlayerController.OnPlayerSpawn += OnPlayerSpawnHandle;
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
            PlayerController.OnPlayerSpawn -= OnPlayerSpawnHandle;
    }
    private void OnPlayerSpawnHandle(PlayerController player)
    {
        player.transform.position = _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
    }
}
