using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class ProjectileShooting : NetworkBehaviour
{

    public GameObject projectile;
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
        ShootServer(Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.rotation);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServer(Vector3 position, Quaternion rotation)
    {
        GameObject spawned = Instantiate(projectile, position, rotation);
        ServerManager.Spawn(spawned);
        
    }
}

