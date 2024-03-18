using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordUI : MonoBehaviour
{
    private TextMeshProUGUI _recordText;

    public ulong clientId;

    public int UserPoint { get; private set; }
    public string UserName { get; private set; }
    public int Rank { get; private set; }

    private void Awake()
    {

        _recordText = GetComponent<TextMeshProUGUI>();
    }
    public void SetOwner(ulong ownerId)
    {
        clientId = ownerId;
    }
    public void SetPoint(int score)
    {
        UserPoint = score;
    }
    public void SetName(string username)
    {
        UserName = username;
    }
    public void SetRank(int rank)
    {
        Rank = rank;
    }

    public void UpdateText()
    {
        _recordText.SetText($"{Rank.ToString()} . {UserName} [{UserPoint.ToString()}]");
    }
}
