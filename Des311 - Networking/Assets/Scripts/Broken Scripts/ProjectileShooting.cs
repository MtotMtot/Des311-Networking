using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class ProjectileShooting : NetworkBehaviour
{

    public GameObject projectile;
    public float timeBetweenFire = 0.6f;
    float fireTimer;
    public Collider playerColl;


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

    // local function call to shoot for client
    private void Shoot()
    {
        ShootServer(Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.rotation); //uses camera position and rotation to spawn projectile in fron of cam
    }

    // Server RPC to spawn the projectile and give it the player's collider
    [ServerRpc(RequireOwnership = false)]
    private void ShootServer(Vector3 position, Quaternion rotation)
    {
        GameObject spawned = Instantiate(projectile, position, rotation);
        ProjectileBehaviour temp = spawned.GetComponent<ProjectileBehaviour>();
        temp.ownerColl = playerColl;
        ServerManager.Spawn(spawned);
        
    }
}

