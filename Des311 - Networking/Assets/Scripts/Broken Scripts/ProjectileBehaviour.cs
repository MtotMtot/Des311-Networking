using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class ProjectileBehaviour : NetworkBehaviour
{
    public int damage;
    public GameObject gameObj;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward*20, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerCollider")
        {
            DealDamage(damage, other.transform);
        }
        else
        {
            Destroy(gameObj);
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void DealDamage(int damage, Transform transform)
    {
        Debug.Log("made it into dealDamageFunc");

        if(transform.TryGetComponent<PlayerHealth>(out PlayerHealth enemyHealth))
        {
            enemyHealth.health -= damage;
            Debug.Log("I hit enemy");
            Destroy(gameObj);
        }
        else
        {
            Destroy(gameObj);
        }
    }
}
