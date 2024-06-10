using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDice : MonoBehaviour
{
    public ChatUser chatUserA;
    public ChatUser chatUserB;

    int chatUserA_value = 0;
    int chatUserB_value = 0;

    public void OnStartDiceBattle()
    {
        StartCoroutine(RollDice());
    }


    IEnumerator RollDice()
    {
        for(int i = 0; i<chatUserA.Dices; i++)
        {
            chatUserA_value += Random.Range(1, 7);
        }

        for (int i = 0; i < chatUserB.Dices; i++)
        {
            chatUserB_value += Random.Range(1, 7);
        }

        yield return new WaitForSeconds(0.5f);

        if(chatUserA_value > chatUserB_value)
        {
            Destroy(chatUserB);
        }
        else
        {
            Destroy(chatUserA);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
