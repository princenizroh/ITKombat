using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
namespace ITKombat
{
    public class RelayManager : MonoBehaviour
    {
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in successfully" + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        private async void CreateRelay()
        {
            try {

                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

                string joincode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                Debug.Log("Join code: " + joincode);

                RelayServerData relayServerData = new RelayServerData(allocation, "dtls" );

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    relayServerData
                    // allocation.RelayServer.IpV4,
                    // (ushort)allocation.RelayServer.Port,
                    // allocation.AllocationIdBytes,
                    // allocation.Key,
                    // allocation.ConnectionData
                    );
            } catch (RelayServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private async void JoinRelay(string joincode)
        {
            try
            {
              Debug.Log("Joining relay with join code: " + joincode);
              JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joincode);
              RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls" );
              NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                  relayServerData
                  );
              // NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
              //     joinAllocation.RelayServer.IpV4,
              //     (ushort)joinAllocation.RelayServer.Port,
              //     joinAllocation.AllocationIdBytes,
              //     joinAllocation.Key,
              //     joinAllocation.ConnectionData,
              //     joinAllocation.HostConnectionData
              //     );

              NetworkManager.Singleton.StartClient();
            } catch (RelayServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

}
