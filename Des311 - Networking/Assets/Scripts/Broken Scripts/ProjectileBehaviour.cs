using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class ProjectileBehaviour : NetworkBehaviour
{
    public int damage;
    public GameObject gameObj;
    private Rigidbody rb;
    private float lifetime = 4;
    public Collider ownerColl;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward*20, ForceMode.Impulse);
    }

    void Update()
    {
        if (!base.IsOwner)
            return;

        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.tag == "PlayerCollider") && (other.gameObject != ownerColl.gameObject)) //deal damage to player
        {
            Debug.Log("Hit player");
            DealDamage(damage, other.gameObject);
        }
        else if (other.tag == "Projectile") //do nothing fi hit self or other projectile
        {
            
        }
        else //destroy projetile if hits anything else
        {
            DestroyProjectile();
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void DealDamage(int damage, GameObject other)
    {

        PlayerHealth enemyHealth = other.GetComponentInParent<PlayerHealth>();
        enemyHealth.health -= damage;
        Debug.Log("I hit enemy");
        Destroy(gameObj);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectile()
    {
        Destroy(gameObj);
    }
}
