using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingScene : MonoBehaviour
{
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private TMP_InputField _portInputField;
    [SerializeField] private TMP_InputField _nameInputField;

    [SerializeField] private Button HostBtn;
    [SerializeField] private Button ClientBtn;

    private void Start()
    {
        string name = PlayerPrefs.GetString("Name", string.Empty);
        if (name != string.Empty)
        {
            _nameInputField.text = name;
        }
        HostBtn.onClick.AddListener(StartHost);
        ClientBtn.onClick.AddListener(StartClient);
    }

    private void StartClient()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            _ipInputField.text,
            (ushort)int.Parse(_portInputField.text)
        );
        PlayerPrefs.SetString("Name", _nameInputField.text);
        ClientSingletone.Instance.GameManager.StartClientAsync();
    }
    private void StartHost()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            _ipInputField.text,
            (ushort)int.Parse(_portInputField.text)
        );
        PlayerPrefs.SetString("Name", _nameInputField.text);
        HostSingletone.Instance.GameManager.StartHostAsync();
    }
}
