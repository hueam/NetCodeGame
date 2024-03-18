using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RankBoardBehaviour : NetworkBehaviour
{
    [SerializeField] private RecordUI _recordPrefab;
    [SerializeField] private RectTransform _recordParentTrm;

    private NetworkList<RankBoardEntityState> _rankList;

    private List<RecordUI> _rankUIList = new List<RecordUI>();

    private void Awake()
    {
        _rankList = new NetworkList<RankBoardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _rankList.OnListChanged += HandleRankListChanged;
            foreach (var entity in _rankList)
            {
                HandleRankListChanged(new NetworkListEvent<RankBoardEntityState>
                {
                    Type = NetworkListEvent<RankBoardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }
        if (IsHost)
        {
            _rankList.OnListChanged += HandleRankListChanged;
            _rankList.Add(new RankBoardEntityState()
            {
                clientId = OwnerClientId,
                playerName = PlayerPrefs.GetString("Name"),
                score = 0
            });
        }

        if (IsServer)
        {
            PlayerController.OnPlayerSpawn += HandleUserJoin;
            PlayerController.OnPlayerDespawn += HandleUserLeft;
        }
    }
    public override void OnNetworkDespawn()
    {
        //여기다 클라도 알잘딱 끊어줘야 한다.
        if (IsClient)
        {
            _rankList.OnListChanged -= HandleRankListChanged;
        }

        if (IsServer)
        {
            PlayerController.OnPlayerSpawn -= HandleUserJoin;
            PlayerController.OnPlayerDespawn -= HandleUserLeft;
        }
    }

    private void HandleUserJoin(PlayerController controller)
    {
        _rankList.Add(new RankBoardEntityState
        {
            clientId = controller.OwnerClientId,
            playerName = controller.playerName,
            score = 0
        });

        controller.point.OnValueChanged += (oldPoint, newPoint) =>
        {
            HandleChangeScore(controller.OwnerClientId, newPoint);
        };
    }

    private void HandleUserLeft(PlayerController controller)
    {
        for (int i = 0; i < _rankList.Count; i++)
        {
            if (_rankList[i].clientId != controller.OwnerClientId) continue;
            Debug.Log(controller.OwnerClientId);
            try
            {
                _rankList.RemoveAt(i);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{_rankList[i].playerName} [{_rankList[i].clientId}] : 삭제중 오류 발행 \n {ex.Message}");
            }
            break;
        }
        controller.point.OnValueChanged = null;
    }
    public void HandleChangeScore(ulong clientId, int score)
    {
        for (int i = 0; i < _rankList.Count; i++)
        {
            if (_rankList[i].clientId != clientId) continue;

            var oldItem = _rankList[i];
            _rankList[i] = new RankBoardEntityState
            {
                clientId = clientId,
                playerName = oldItem.playerName,
                score = score
            };
            break;
        }
    }

    private void HandleRankListChanged(NetworkListEvent<RankBoardEntityState> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<RankBoardEntityState>.EventType.Add:
                AddUIToList(evt.Value);
                break;
            case NetworkListEvent<RankBoardEntityState>.EventType.RemoveAt:
                Debug.Log("스위치");
                RemoveFromUIList(evt.Value.clientId);
                break;
            case NetworkListEvent<RankBoardEntityState>.EventType.Value:
                AdjustScoreToUIList(evt.Value);
                break;
        }
        SortOrder();
    }

    private void AdjustScoreToUIList(RankBoardEntityState value)
    {
        //값을 받아서 해당 UI를 찾아서 socre 갱신
        // 선택 : 갱신후에는 
        for (int i = 0; i < _rankUIList.Count; i++)
        {
            if (_rankUIList[i].clientId == value.clientId)
            {
                _rankUIList[i].SetPoint(value.score);
                return;
            }
        }
    }

    private void AddUIToList(RankBoardEntityState value)
    {
        var target = _rankUIList.Find(x => x.clientId == value.clientId);
        if (target != null) return;

        RecordUI newUI = Instantiate(_recordPrefab, _recordParentTrm);
        newUI.SetOwner(value.clientId);
        newUI.SetRank(_rankList.Count);
        newUI.SetName(value.playerName.ToString());
        newUI.SetPoint(value.score);
        newUI.UpdateText();
        _rankUIList.Add(newUI);
    }
    private void RemoveFromUIList(ulong clientId)
    {
        print("실행");
        var target = _rankUIList.Find(x => x.clientId == clientId);
        if (target != null)
        {
            _rankUIList.Remove(target);
            Destroy(target.gameObject);
            return;
        }
    }

    public void SortOrder()
    {
        // b-a : 내림, a-b : 오름
        _rankUIList.Sort((a, b) => b.UserPoint.CompareTo(a.UserPoint));

        for (int i = 0; i < _rankUIList.Count; ++i)
        {
            var item = _rankUIList[i];
            item.SetRank(i + 1); //등수 기록하고
            //item.Root.BringToFront();
            item.UpdateText();

            if (i < 4)
            {
                item.transform.SetSiblingIndex(i);
            }
            //표현해야할 수보다 작으면 표시

            //자기꺼는 무조건 표시
            if (i > 3 && item.clientId == NetworkManager.Singleton.LocalClientId)
            {
                item.transform.SetSiblingIndex(4);
            }
        }



    }
}
