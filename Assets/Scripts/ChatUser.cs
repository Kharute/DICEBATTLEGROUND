using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUser : NetworkBehaviour
{
    [SyncVar]
    public string PlayerName;

    public int Dices = 1;

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
        Dices++;
    }
}
