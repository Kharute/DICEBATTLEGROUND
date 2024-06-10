using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using Cinemachine;
using TMPro;

public class NetSpawnedObject : NetworkBehaviour
{
    [Header("Components")]
    
    public NavMeshAgent NavAgent_Player;
    public Animator Animator_Player;
    public TextMesh TextMesh_HealthBar;
    public Transform Transform_Player;
    CinemachineVirtualCamera VirtualCamera_Player;

    [Header("Movement")]
    public float _rotationSpeed = 100.0f;

    [Header("Attack")]
    public KeyCode _atkKey = KeyCode.Space;

    public GameObject Prefab_AtkObject;
    public Transform Tranfrom_AtkSpawnPos;

    [Header("Chat")]
    public KeyCode _chatKey = KeyCode.Tab;

    private void Start()
    {
        FollowTheCameraToPlayer();
        
    }
    private void Update()
    {
        if (CheckIsFocusedOnUpdate() == false)
        {
            return;
        }

        CheckIsLocalPlayerOnUpdate();
    }

    void FollowTheCameraToPlayer()
    {
        VirtualCamera_Player = FindObjectOfType<CinemachineVirtualCamera>();
        if (isLocalPlayer)
        {
            if (VirtualCamera_Player != null)
                VirtualCamera_Player.Follow = transform;
        }
    }
    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isFocused;
    }

    private void CheckIsLocalPlayerOnUpdate()
    {
        if (isLocalPlayer == false)
            return;


        // 회전
        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontal * _rotationSpeed * Time.deltaTime, 0);

        // 이동
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        NavAgent_Player.velocity = forward * Mathf.Max(vertical, 0) * NavAgent_Player.speed;
        Animator_Player.SetBool("Moving", NavAgent_Player.velocity != Vector3.zero);

        // 공격
        if (Input.GetKeyDown(_atkKey))
        {
            CommandAtk();
        }
        if (Input.GetKeyDown(_chatKey))
        {
            var _chattingUI = FindAnyObjectByType<ChattingUI>();
            if (_chattingUI != null)
            {
                _chattingUI.transform.localScale = (_chattingUI.transform.localScale == Vector3.one ? Vector3.zero : Vector3.one);
            }
        }
    }

    // 서버 사이드
    [Command]
    void CommandAtk()
    {
        GameObject atkObjectForSpawn = Instantiate(Prefab_AtkObject, Tranfrom_AtkSpawnPos.position, Tranfrom_AtkSpawnPos.transform.rotation);
        atkObjectForSpawn.GetComponent<NetSpawnedSubObject>()._user = gameObject.GetComponent<ChatUser>();
        NetworkServer.Spawn(atkObjectForSpawn);

        RpcOnAtk();
    }

    [ClientRpc]
    void RpcOnAtk()
    {
        Animator_Player.SetTrigger("Atk");
    }
}
