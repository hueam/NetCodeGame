using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    public Action<UserData, ulong> OnClientLeft;
    public Action<UserData, ulong> OnClientJoin;

    private Dictionary<ulong, UserData> _clientToAuthDictionary = new Dictionary<ulong, UserData>();

    private NetworkObject _playerPrefab;

    public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab)
    {
        _networkManager = networkManager;

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;

        _networkManager.OnServerStarted += OnNetworkReady;
        _playerPrefab = playerPrefab;
    }

    //클라이언트들이 서버에 접속할 때 실행을 시켜줘서 요청에 따라 승인응답할 수도있고 안할수도있어
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req,
                                    NetworkManager.ConnectionApprovalResponse res)
    {
        UserData data = new UserData();
        data.Deserialize(req.Payload);
        _clientToAuthDictionary[req.ClientNetworkId] = data;
        foreach (var item in _clientToAuthDictionary)
        {
            Debug.Log(item.Value);
        }
        OnClientJoin?.Invoke(data, req.ClientNetworkId);

        res.Approved = true;
        res.CreatePlayerObject = true;
        //이나블 되어있는 모든 위치중에 랜덤한 위치가 나온다.
        //호스트는 랜덤 포지션을 다 가져오기도 전에 들어와버려 
        //var pos = TankSpawnPoint.GetRandomSpawnPos();
        //res.Position = pos;
        //Vector3 dir = Vector3.zero - pos;
        //float angle = Mathf.Atan2(dir.y, dir.x) - 90f;
        //res.Rotation = Quaternion.identity;
        //res.CreatePlayerObject = true;
    }

    //이녀석을 만들기 위한 정보를 줘야 한다.
    public void SpawnPlayer(ulong clientID)
    {
        NetworkObject player = GameObject.Instantiate(_playerPrefab);
        player.SpawnAsPlayerObject(clientID);

        //해당하는 플레이어 오브젝트를 clientID에게 오너로 할당하고
    }

        //    TankPlayer tankComponent = player.GetComponent<TankPlayer>();
        //    //여기다가 값들을 셋팅하고 RPC를 날려서 다같이 셋팅하도록 해야해.
        //    tankComponent.SetTankNetworkVariable(userState);
        //    //해당 플레이어에 선택한 값들을 적용시킨다.
        //    tankComponent.SetTankVisualClientRpc(clientID);
        //}


        private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (_clientToAuthDictionary.TryGetValue(clientId, out UserData data))
        {
            _clientToAuthDictionary.Remove(clientId);
            OnClientLeft?.Invoke(data, clientId); //클라이언트 접속 종료시에 알려준다.
        }
    }

    public void Dispose()
    {
        if (_networkManager == null) return;

        _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        _networkManager.OnServerStarted -= OnNetworkReady;
        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (_clientToAuthDictionary.TryGetValue(clientId, out UserData data))
        {
            return data;
        }
        return null;
    }
}
