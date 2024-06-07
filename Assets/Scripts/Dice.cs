using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    Collider cd;
    Rigidbody rb;

    private void Awake()
    {
        cd = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            rb.ResetCenterOfMass();
        }
        if (collision.collider.CompareTag("Player"))
        {
            ChatUser player = collision.collider.GetComponent<ChatUser>();
            player.AddDice();

            gameObject.SetActive(false);
        }
    }

}

