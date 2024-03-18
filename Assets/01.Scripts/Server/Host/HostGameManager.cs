
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private const string GameScenename = "Game";

    public NetworkServer NetworkServer { get; private set; }

    private NetworkObject _playerPrefab;
    public HostGameManager(NetworkObject playerPrefab)
    {
        _playerPrefab = playerPrefab;
    }

    public void ShutdownAsync()
    {
        NetworkServer?.Dispose();
    }

    public void Dispose()
    {
        ShutdownAsync();
    }

    public void StartHostAsync()
    {
        

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        string playerName = PlayerPrefs.GetString("Name", "Unknown");

        NetworkServer = new NetworkServer(NetworkManager.Singleton, _playerPrefab);

        //여기다가 데이터를 같이 보낸다
        UserData userData = new UserData
        {
            username = playerName,
        };

        NetworkManager.Singleton.NetworkConfig.ConnectionData = userData.Serialize().ToArray();

        if (NetworkManager.Singleton.StartHost())
            NetworkManager.Singleton.SceneManager.LoadScene(GameScenename, LoadSceneMode.Single);
        else
            NetworkManager.Singleton.Shutdown();
    }
}
