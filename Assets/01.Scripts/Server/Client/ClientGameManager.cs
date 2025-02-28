using System;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private const string MenuScenename = "Setting";
    private NetworkClient _networkClient;
    public string name;

    public void InitAsync()
    {

        _networkClient = new NetworkClient(NetworkManager.Singleton);
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene(MenuScenename);
    }

    public void StartClientAsync()
    {
        //여기다가 데이터를 같이 보낸다
        UserData userData = new UserData
        {
            username = PlayerPrefs.GetString("Name", "Unknown"),
        };

        NetworkManager.Singleton.NetworkConfig.ConnectionData = userData.Serialize().ToArray();

        NetworkManager.Singleton.StartClient();
            //SceneManager.LoadScene("Game", LoadSceneMode.Single);
        //else
        //    NetworkManager.Singleton.Shutdown();
    }

    public void Dispose()
    {
        _networkClient?.Dispose();
    }

    public void Disconnect()
    {
        _networkClient.Disconnect();
    }
}
