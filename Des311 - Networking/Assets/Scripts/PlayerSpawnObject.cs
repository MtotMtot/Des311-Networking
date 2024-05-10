using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerSpawnObject : NetworkBehaviour
{
    public GameObject objToSpawn;
    [HideInInspector] public GameObject spawnedObject;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            GetComponent<PlayerSpawnObject>().enabled = false;
    }

    private void Update()
    {
        //spawns obj with local reference to it
        if (spawnedObject == null && Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnObject(objToSpawn, transform, this);
        }

        //spawns despawns object if it is spawned (isnt null)
        if (spawnedObject != null && Input.GetKeyDown(KeyCode.Alpha2))
        {
            DespawnObject(spawnedObject);
        }
    }

    // spawns object infront of player on client, then spawns it on server, then spawns it for all clients
    [ServerRpc]
    public void SpawnObject(GameObject obj, Transform player, PlayerSpawnObject script)
    {
        GameObject spawned = Instantiate(obj, player.position + player.forward, Quaternion.identity);
        ServerManager.Spawn(spawned); //spawned object can be assigned owners when spawn with Spawn.(spawned, owner)
        SetSpawnedObject(spawned, script);
    }

    // spawns object for oberservers
    [ObserversRpc]
    public void SetSpawnedObject(GameObject spawned, PlayerSpawnObject script)
    {
        script.spawnedObject = spawned;
    }

    // doesnt need to own the spawned object to interact with it. Since spawned object is stored on client, no ownership is needed for function
    [ServerRpc(RequireOwnership = false)]
    public void DespawnObject(GameObject obj)
    {
        ServerManager.Despawn(obj);
    }
}
