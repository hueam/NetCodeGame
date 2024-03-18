using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using System;

public class PlayerController : NetworkBehaviour
{
    public static event Action<PlayerController> OnPlayerSpawn;
    public static event Action<PlayerController> OnPlayerDespawn;

    public float ChargingSpeed = 2f;
    [SerializeField] private Transform _footPositionTrm;
    [SerializeField] private Transform _jumpPivot;
    [SerializeField] private Transform _arrow;
    [SerializeField] private float _pushRadius = 2f;
    [SerializeField] CinemachineVirtualCamera playerVirtualCam;
    [SerializeField] private LayerMask _whatIsEnemy;
    [SerializeField] private LayerMask _whatIsGround;

    private Rigidbody2D _rigid;

    private bool _isCharging;
    private float _chargingPower;
    private bool _isStop;
    private bool _isGround;
    private Vector2 forceDirection;

    private PlayerAnimater _animater;

    public string playerName;

    Camera mainCam;
    Camera MainCam
    {
        get
        {
            if (mainCam == null)
                mainCam = Camera.main;
            return mainCam;
        }
    }
    [SerializeField] private float _dropPower;

    public NetworkVariable<int> point = new NetworkVariable<int>();


    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _animater = transform.Find("Visual").GetComponent<PlayerAnimater>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            playerVirtualCam.Priority = 20;
        if (IsServer)
        {
            UserData data = HostSingletone.Instance.GameManager.NetworkServer
                                            .GetUserDataByClientId(OwnerClientId);
            playerName = data.username;


            OnPlayerSpawn?.Invoke(this);
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawn?.Invoke(this);
        }
    }
    private void Update()
    {
        if (!IsOwner) return;

        _animater.SetYVelocity(_rigid.velocity.y);
        _animater.SetIsJump(!_isGround);
        if (_isCharging == true)
        {
            _chargingPower = Mathf.Clamp((_chargingPower += Time.deltaTime * ChargingSpeed), 0f, 4f);
            _arrow.localScale = new Vector3(_chargingPower * 3, 1, 1);
        }
        _isStop = _rigid.velocity.sqrMagnitude < 0.2;

        if (Input.GetMouseButtonDown(0) && _isStop)
        {
            StartCharging();
        }

        if (Input.GetMouseButtonUp(0) && _isCharging)
        {
            EndCharging();
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGround == false)
        {
            Drop();
        }

        Vector2 mousePos = MainCam.ScreenToWorldPoint(Input.mousePosition);
        forceDirection = mousePos - (Vector2)_jumpPivot.position;
        _jumpPivot.right = forceDirection;
        float forceDir = mousePos.x - transform.position.x;
        _animater.Filp(forceDir > 0);
    }
    private void LateUpdate()
    {
        _isGround = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, _whatIsGround);
    }
    private void StartCharging()
    {
        _isCharging = true;
    }

    private void EndCharging()
    {
        _animater.SetIsJump(true);
        Vector2 mousePos = MainCam.ScreenToWorldPoint(Input.mousePosition);
        forceDirection = mousePos - (Vector2)_jumpPivot.position;
        _isCharging = false;
        _rigid.AddForce(forceDirection * _chargingPower, ForceMode2D.Impulse);

        _chargingPower = 0;
        _arrow.localScale = new Vector3(_chargingPower, 1, 1);
        //EndChargingServerRpc(forceDir);
    }


    private void Drop()
    {
        _rigid.velocity = new Vector2(0, -_dropPower);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        //if (!IsServer) return;

        if (other.transform.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (transform.position.y < other.transform.position.y) return;
            //PlayerController higher = other.transform.position.y > transform.position.y ? controller : this;
            //PlayerController lower = other.transform.position.y > transform.position.y ? this : controller;

            CastFootPushServerRPC(controller.OwnerClientId);
            //bool result = CastFootPush(controller.OwnerClientId);
        }
    }

    public bool CastFootPush(ulong targetClientID)
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(_footPositionTrm.position, _pushRadius, _whatIsEnemy);

        foreach (Collider2D collider in others)
        {
            if (collider.TryGetComponent<PlayerController>(out PlayerController target))
            {
                if (target.OwnerClientId == targetClientID)
                {
                    target.gameObject.SetActive(false);
                    return true;
                }
            }
        }
        return false;
    }
    [ServerRpc(RequireOwnership = false)]
    public void CastFootPushServerRPC(ulong targetClientID)
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(_footPositionTrm.position, _pushRadius, _whatIsEnemy);

        foreach (Collider2D collider in others)
        {
            if (collider.TryGetComponent<PlayerController>(out PlayerController target))
            {
                if (target.OwnerClientId == targetClientID)
                {
                    point.Value++;
                    Destroy(target.gameObject);
                    break;
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_footPositionTrm == null) return;
        Gizmos.DrawWireSphere(_footPositionTrm.position, _pushRadius);
    }
#endif
}
