using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUser : NetworkBehaviour
{
    [SyncVar]
    public string PlayerName;

    [SyncVar]
    private int _dices = 1;

    public int Dices
    {
        get { return _dices; }
        set { _dices = value; }
    }

    public override void OnStartServer()
    {
        PlayerName = (string)connectionToClient.authenticationData;
    }

    public override void OnStartLocalPlayer()
    {
        var chattingUI = FindObjectOfType<ChattingUI>();

        if (chattingUI != null)
        {
            chattingUI.SetLocalPlayerName(PlayerName);
        }
    }
    public void AddDice()
    {
        _dices++;
    }
}
