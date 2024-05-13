using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using FishNet.Object;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar] public int health = 10;
    private TextMeshProUGUI healthText;
    public GameObject playerObj;

    private void Start()
    {
        healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;

        healthText.text = health.ToString();

        if(health <= 0)
        {
            DestroyPlayer();
        }
    }

    [ServerRpc]
    private void DestroyPlayer()
    {
        Destroy(playerObj);
    }
}