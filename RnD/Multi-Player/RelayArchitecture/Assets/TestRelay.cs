using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;


public class TestRelay : MonoBehaviour
{

    public Button hostButton;
    public Button clientButton;

    
    private string joinCode;

    private void Awake()
    {
        hostButton.onClick.AddListener(CreateRelay);
        clientButton.onClick.AddListener(JoinRelay);
    }
    private async void Start()
    {

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + " " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    public async void CreateRelay()
    {
        try {

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join code is " + " " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            await File.WriteAllTextAsync("joinCode.txt", joinCode);
            Debug.Log("The join code has been saved.");

        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }

    }
   
    private async void JoinRelay()
    {
        try
        {
            string joinCode = File.ReadAllText("joinCode.txt");

            Debug.Log("Joining relay with " + " " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();

        }catch(RelayServiceException e)
        {
            Debug.Log(e);
        }


    }
}
