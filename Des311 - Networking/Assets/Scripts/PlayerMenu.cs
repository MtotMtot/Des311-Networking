using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using System.Threading.Tasks;
using FishNet.Managing;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using FishNet.Transporting.UTP;
using TMPro;

public class PlayerMenu : MonoBehaviour
{
    //netowrk manager and relay vars
    [SerializeField] private NetworkManager _networkManager;
    public TMP_Text jCodeText;

    // input field vars for relay
    [Header("The value got from input fields")]
    [SerializeField] private string inputJoinCode;
    [SerializeField] private int inputMaxConnections;

    //inGame bool check for menuscripts
    public bool inGame = false;

    //main menu GameObj reference
    [SerializeField] private GameObject mainMenu;

    public void GrabFromJoinCodeInput(string input)
    {
        inputJoinCode = input;
    }

    public void GrabFromHostInput(string input)
    {
        string stringInput = input;
        inputMaxConnections = System.Convert.ToInt32(stringInput) - 1 ;
    }

    public void JoinClient()
    {
        StartClientWithRelay(inputJoinCode);
    }

    public void JoinHost()
    {
        StartHostWithRelay(inputMaxConnections);
    }

    void Start()
    {
        

    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.J))
        {
            StartClientWithRelay(jCode);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartHostWithRelay(3);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.developerConsoleVisible = true;
            Debug.Log("console enabled");
        }*/

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mainMenu.SetActive(!mainMenu.activeSelf);
            Cursor.visible = !(Cursor.visible);
            if(Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }

    public void ConvertInputToInt()
    {
        
    }


    public async void StartHostWithRelay(int maxConnections = 5)
    {
        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always authenticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Request allocation and join code
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        // Configure transport
        var unityTransport = _networkManager.TransportManager.GetTransport<FishyUnityTransport>();
        unityTransport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

        // Start host
        if (_networkManager.ServerManager.StartConnection()) // Server is successfully started.
        {
            //Start client
            _networkManager.ClientManager.StartConnection();
            jCodeText.text = joinCode;
            inGame = true;
        }
        
    }

    public async void StartClientWithRelay(string joinCode)
    {
        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always authenticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Join allocation
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        // Configure transport
        var unityTransport = _networkManager.TransportManager.GetTransport<FishyUnityTransport>();
        unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        // Start client
        Debug.Log( !string.IsNullOrEmpty(joinCode) && _networkManager.ClientManager.StartConnection() );
        inGame = true;
    }
}
