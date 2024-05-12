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
    [SerializeField] private NetworkManager _networkManager;
    public string jCode;
    public TMP_Text jCodeText;

    void Start()
    {
        

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
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
        }

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
            _networkManager.ClientManager.StartConnection();
            jCode = joinCode;
            jCodeText.text = joinCode;
            Debug.Log(jCode);
        }
        jCode =  null;
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
    }
}
