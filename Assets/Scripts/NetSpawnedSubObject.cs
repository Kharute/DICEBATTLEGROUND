using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetSpawnedSubObject : NetworkBehaviour
{
    public float _destroyAfter = 1.0f;


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
        DestroySelf();
    }
}
