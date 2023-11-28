using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{
    public float ChargingSpeed = 2f;
    [SerializeField] CinemachineVirtualCamera playerVirtualCam;
    private Rigidbody2D _rigid;

    private bool _isCharging;
    private float _chargingTimer;
    private bool _isStop;

    private PlayerAnimater _animater;

    Camera mainCam;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _animater = GetComponent<PlayerAnimater>();

        mainCam = Camera.main;
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            playerVirtualCam.Priority = 20;
    }
    private void Update()
    {
            _animater.SetYVelocity(_rigid.velocity.y);
        if (_isCharging == true)
        {
            _chargingTimer = Mathf.Clamp((_chargingTimer += Time.deltaTime * ChargingSpeed), 0f, 4f);
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
    }
    private void StartCharging()
    {
        _chargingTimer = 0;
        _isCharging = true;
    }
    private void EndCharging()
    {
        _animater.SetIsJump(true);
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mousePos);
        Vector2 forceDir = mousePos - (Vector2)transform.position;
        _rigid.AddForce(forceDir * _chargingTimer, ForceMode2D.Impulse);
        _isCharging = false;
    }
}
