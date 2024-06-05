using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingManager : NetworkManager
{
    [SerializeField] StartUI _StartUI;
    [SerializeField] ChattingUI _chattingUI;

    public void OnInputValueChanged_SetHostName(string hostName)
    {
        this.networkAddress = hostName;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (_chattingUI != null)
        {
            _chattingUI.RemoveNameOnServerDisconnect(conn);
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        if (_StartUI != null)
        {
            _StartUI.SetUIOnClientDisconnected();
        }
    }
}
