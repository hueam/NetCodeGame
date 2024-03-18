using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ApplicationController : MonoBehaviour
{
    //[SerializeField] private HostSingleton _hostPrefab;
    //[SerializeField] private ClientSingleton _clientPrefab;



    [SerializeField] private ClientSingletone _clientPrefab;
    [SerializeField] private HostSingletone _hostPrefab;
    [SerializeField] private NetworkObject _playerPrefab;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        HostSingletone hostSingletone = Instantiate(_hostPrefab);
        hostSingletone.CreateHost(_playerPrefab); //게임매니저 만들고 준비

        ClientSingletone clientSingletone = Instantiate(_clientPrefab);
        clientSingletone.CreateClient();

        clientSingletone.GameManager.GotoMenu();
    }


}
