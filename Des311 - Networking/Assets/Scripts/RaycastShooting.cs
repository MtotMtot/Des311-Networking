using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class RaycastShooting : NetworkBehaviour
{
    public int damage;
    public LayerMask playerLayer;

    public float timeBetweenFire;
    float fireTimer;


    private void Update()
    {
        if (!base.IsOwner)
            return;

        if (Input.GetButton("Fire1"))
        {
            if (fireTimer <= 0)
            {
                Shoot();
                fireTimer = timeBetweenFire;
            }
        }

        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;
    }

    private void Shoot()
    {
        ShootServer(damage, Camera.main.transform.position, Camera.main.transform.forward);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServer(int damageToGive, Vector3 position, Vector3 direction)
    {
        if (Physics.Raycast(position, direction, out RaycastHit hit) && hit.transform.CompareTag("PlayerCollider"))
        {
            PlayerHealth enemyHealth = hit.transform.gameObject.GetComponentInParent<PlayerHealth>();
            Debug.Log("passed rayhit and componet check");
            enemyHealth.health -= damageToGive;
        }
    }
}

