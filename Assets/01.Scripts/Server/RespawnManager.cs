using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnManager : NetworkBehaviour
{
    [SerializeField] NetworkObject _playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            PlayerController.OnPlayerDespawn += HandlePlayerDespawn;
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
            PlayerController.OnPlayerDespawn -= HandlePlayerDespawn;
    }
    public void HandlePlayerDespawn(PlayerController player)
    {
        StartCoroutine(DelayRespawn(player.OwnerClientId));
    }
    IEnumerator DelayRespawn(ulong id)
    {
        yield return new WaitForSeconds(3f);
        if (HostSingletone.Instance.GameManager.NetworkServer.GetUserDataByClientId(id) != null)
        {
            var playerObj = GameObject.Instantiate(_playerPrefab);
            playerObj.SpawnAsPlayerObject(id);
        }
    }
}
