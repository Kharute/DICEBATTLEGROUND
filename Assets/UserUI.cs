using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserUI : MonoBehaviour
{

    TextMeshProUGUI Dice_Count;

    private void Awake()
    {
        ChatUser[] blj = FindObjectsByType<ChatUser>(FindObjectsSortMode.None);
        foreach (ChatUser u in blj)
        {
            //if (u.name == (string)connectionToClient.authenticationData) { }
        }
    }

    /*private void Update()
{
   OnDiceUpdate();
}

[Client]
private void OnDiceUpdate()
{
   //var string = GetAuthenticator().authenticationData as string;
   if (connectionToClient.authenticationData != null)
   {
       if (Dice_Count != null)
           Dice_Count.text = string.Empty;
   }

}*/
}
