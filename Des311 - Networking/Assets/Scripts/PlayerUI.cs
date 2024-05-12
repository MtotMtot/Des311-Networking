using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    //sub menus reference
    public GameObject mainMenu;
    public GameObject hud;


    //relayScript reference
    public PlayerMenu relayScript;

    void Start()
    {
        hud.SetActive(false);


        relayScript = GameObject.FindGameObjectWithTag("RelayManager").GetComponent<PlayerMenu>();
    }

    void Update()
    {
        if (relayScript.inGame)
        {
            hud.SetActive(true);

            mainMenu.SetActive(false);

            relayScript.inGame = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    
}
