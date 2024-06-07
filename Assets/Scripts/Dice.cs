using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VHierarchy.Libs;

public class Dice : MonoBehaviour
{
    Collider cd;
    Rigidbody rb;

    private void Awake()
    {
        cd = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            rb.ResetCenterOfMass();
            rb.Destroy();
            cd.isTrigger = true;
        }
    }
}
