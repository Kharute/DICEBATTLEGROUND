using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using Cinemachine;

public class NetSpawnedObject : NetworkBehaviour
{
    [Header("Components")]
    
    public NavMeshAgent NavAgent_Player;
    public Animator Animator_Player;
    public TextMesh TextMesh_HealthBar;
    public Transform Transform_Player;
    CinemachineBrain CB;
    CinemachineVirtualCamera VirtualCamera_Player;
    //CinemachineFreeLook FreeLook_Player;


    [Header("Movement")]
    public float _rotationSpeed = 100.0f;
    public Vector3 MoveDir { get; private set; }
    Vector2 dir;

    [Header("Attack")]
    public KeyCode _atkKey = KeyCode.Space;
    public GameObject Prefab_AtkObject;
    public Transform Tranfrom_AtkSpawnPos;

    [Header("Stats Server")]
    [SyncVar] public int _health = 4;

    public CinemachineVirtualCamera Camera_Mine;

    private void Start()
    {
        VirtualCamera_Player = FindObjectOfType<CinemachineVirtualCamera>();
        if (isLocalPlayer)
        {
            if (VirtualCamera_Player != null)
                VirtualCamera_Player.Follow = transform;
        }
    }
    private void Update()
    {
        //Camera_Mine.gameObject.SetActive(true);
        //SetHealthBarOnUpdate(_health);

        if (CheckIsFocusedOnUpdate() == false)
        {
            return;
        }

        CheckIsLocalPlayerOnUpdate();
    }

    /*private void SetHealthBarOnUpdate(int health)
    {
        TextMesh_HealthBar.text = new string('-', health);
    }*/
    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isFocused;
    }

    private void CheckIsLocalPlayerOnUpdate()
    {
        if (isLocalPlayer == false)
            return;

        //SetPlayerCinemachine();

        //PlayerMove();

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

        //RotateLocalPlayer();
    }

    void PlayerMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        MoveDir = dir.y * (Camera.main.transform.forward * horizontal) + dir.x * (Camera.main.transform.right * vertical);

        //MoveDir = dir.y * Camera.main.transform.forward + dir.x * Camera.main.transform.right;
        MoveDir = new Vector3(MoveDir.x, 0, MoveDir.z);
        MoveDir.Normalize();
        MoveDir *= NavAgent_Player.speed * Time.deltaTime;

        if (MoveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MoveDir), _rotationSpeed * Time.deltaTime);
        }
    }

    // 서버 사이드
    [Command]
    void CommandAtk()
    {
        GameObject atkObjectForSpawn = Instantiate(Prefab_AtkObject, Tranfrom_AtkSpawnPos.transform.position, Tranfrom_AtkSpawnPos.transform.rotation);
        NetworkServer.Spawn(atkObjectForSpawn);

        RpcOnAtk();
    }

    [ClientRpc]
    void RpcOnAtk()
    {
        Animator_Player.SetTrigger("Atk");
    }

    // 클라에서 다음 함수가 실행되지 않도록 ServerCallback을 달아줌
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var atkGenObject = other.GetComponent<NetSpawnedSubObject>();
        if (atkGenObject != null)
        {
            _health--;

            if (_health == 0)
            {
                NetworkServer.Destroy(this.gameObject);
            }
        }
    }

    /*void RotateLocalPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.DrawLine(ray.origin, hit.point);
            Vector3 lookRotate = new Vector3(hit.point.x, Transform_Player.position.y, hit.point.z);
            Transform_Player.LookAt(lookRotate);
        }
    }*/

    /*[Client]
    void SetPlayerCinemachine()
    {
        CB = FindAnyObjectByType<CinemachineBrain>();
        CB.IsLiveInBlend(VirtualCamera_Player);
        //CB. = VirtualCamera_Player;
    }*/
}
