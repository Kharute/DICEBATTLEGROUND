using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetSpawnedSubObject : NetworkBehaviour
{
    public float _destroyAfter = 1.0f;

    ChatUser _user;

    [SerializeField]
    BattleDice _battleDice;

    private void Awake()
    {
        _user = GetComponentInParent<GameObject>().GetComponent<ChatUser>();
    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), _destroyAfter);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ChatUser _cUser = other.GetComponent<ChatUser>();

            if(_cUser != _user && _cUser != null)
            {
                _battleDice.chatUserA = _user;
                _battleDice.chatUserB = _user;

                _battleDice.OnStartDiceBattle();
            }
        }
        DestroySelf();
    }
}
