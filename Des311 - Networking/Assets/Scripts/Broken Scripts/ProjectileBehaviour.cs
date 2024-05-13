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
        if((other.tag == "PlayerCollider") && (other.gameObject != ownerColl.gameObject)) //only deal damage if hitting other players, not owner
        {
            Debug.Log("Hit player");
            DealDamage(damage, other.gameObject);
        }
        else if (other.tag == "Projectile") //do nothing if hit self or other projectile
        {
            
        }
        else //destroy projetile if hits anything else
        {
            DestroyProjectile();
        }
        
    }

    // Server RPC for dealing damage to target
    [ServerRpc(RequireOwnership = false)]
    private void DealDamage(int damage, GameObject other)
    {

        PlayerHealth enemyHealth = other.GetComponentInParent<PlayerHealth>();
        enemyHealth.health -= damage;
        Debug.Log("I hit enemy");
        Destroy(gameObj);
    }

    // Server RPC for destroying projectile (not done locally as projectile can destroy itself before deal damage function has been completed)
    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectile()
    {
        Destroy(gameObj);
    }
}
