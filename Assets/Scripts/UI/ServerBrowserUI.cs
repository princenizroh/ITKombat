using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System;

namespace ITKombat
{
    public class ServerBrowserUI : MonoBehaviour {

    [SerializeField] private Transform serverContainer;
    [SerializeField] private Transform serverTemplate;
    [SerializeField] private Button joinIPButton;
    [SerializeField] private Button createServerButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;


    private void Awake() {
        joinIPButton.onClick.AddListener(() => {
            string ipv4Address = ipInputField.text;
            ushort port = ushort.Parse(portInputField.text);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);
            MultiplayerManager.Instance.StartClient();
        });

        createServerButton.onClick.AddListener(() => {
            string keyId = "f20b5c5d-8050-4add-9dd1-667b0383d6b6";
            string keySecret = "XNRN5Yd3M_-CBhOtorcakcYqM-SvKpdl";
            byte[] keyByteArray = Encoding.UTF8.GetBytes(keyId + ":" + keySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);

            Debug.Log("Base64 Encoded Key: " + keyBase64);

            string projectId = "e8e0926d-9da6-4c43-bc2f-5cb175dc23a7";
            string environmentId = "1c2edc9d-6d14-4362-9843-96cadb1fa42f";
            string url = $"https://services.api.unity.com/auth/v1/token-exchange?projectId={projectId}&environmentId={environmentId}";

            Debug.Log("Request URL: " + url);

            string jsonRequestBody = JsonUtility.ToJson(new TokenExchangeRequest {
                scopes = new[] { "multiplay.allocations.create", "multiplay.allocations.list" },
            });

            Debug.Log("Request Body JSON: " + jsonRequestBody);

            WebRequests.PostJson(url,
            (UnityWebRequest unityWebRequest) => {
                unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64);
                Debug.Log("Authorization Header: " + unityWebRequest.GetRequestHeader("Authorization"));
            },
            jsonRequestBody,
            (string error) => {
                Debug.Log("Error: " + error);
                // Menampilkan detail tambahan dari unityWebRequest jika error terjadi
            },
            (string json) => {
                Debug.Log("Token Exchange Success: " + json);
                TokenExchangeResponse tokenExchangeResponse = JsonUtility.FromJson<TokenExchangeResponse>(json);

                string fleetId = "72667efe-5877-4776-9017-e3f515687cfa";
                string allocationUrl = $"https://multiplay.services.api.unity.com/v1/allocations/projects/{projectId}/environments/{environmentId}/fleets/{fleetId}/allocations";

                Debug.Log("Allocation Request URL: " + allocationUrl);

                WebRequests.PostJson(allocationUrl,
                (UnityWebRequest unityWebRequest) => {
                    unityWebRequest.SetRequestHeader("Authorization", "Bearer " + tokenExchangeResponse.accessToken);
                    Debug.Log("Authorization Bearer Token: " + unityWebRequest.GetRequestHeader("Authorization"));
                    Debug.Log("Queue Allocation Request Body: " + JsonUtility.ToJson(new QueueAllocationRequest {
                        allocationId = "dee1fe8f-42a9-4dc4-8ecc-67727a0b2471",
                        buildConfigurationId = 0,
                        regionId = "8585763",
                    }));
                },
                JsonUtility.ToJson(new QueueAllocationRequest {
                    allocationId = "dee1fe8f-42a9-4dc4-8ecc-67727a0b2471",
                    buildConfigurationId = 0,
                    regionId = "8585763",
                }),
                (string error) => {
                    Debug.Log("Allocation Error: " + error);
                    // Menampilkan detail tambahan dari unityWebRequest jika error terjadi
                },
                (string json) => {
                    Debug.Log("Allocation Success: " + json);
                });
            });
        });

        serverTemplate.gameObject.SetActive(false);
        foreach (Transform child in serverContainer) {
            if (child == serverTemplate) continue;
            Destroy(child.gameObject);
        }
    }


#if !DEDICATED_SERVER
    private void Start() {
        string keyId = "f20b5c5d-8050-4add-9dd1-667b0383d6b6";
        string keySecret = "XNRN5Yd3M_-CBhOtorcakcYqM-SvKpdl";
        byte[] keyByteArray = Encoding.UTF8.GetBytes(keyId + ":" + keySecret);
        string keyBase64 = Convert.ToBase64String(keyByteArray);

        string projectId = "e8e0926d-9da6-4c43-bc2f-5cb175dc23a7";
        string environmentId = "1c2edc9d-6d14-4362-9843-96cadb1fa42f";
        string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{projectId}/environments/{environmentId}/servers";

        Debug.Log("Starting Server Request...");
        Debug.Log("Request URL: " + url);
        Debug.Log("Base64 Encoded Key: " + keyBase64);

        WebRequests.Get(url,
        (UnityWebRequest unityWebRequest) => {
            unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64);
            Debug.Log("Authorization Header Set: " + unityWebRequest.GetRequestHeader("Authorization"));
        },
        (string error) => {
            Debug.Log("Error retrieving server list: " + error);
        },
        (string json) => {
            Debug.Log("Server list retrieved successfully: " + json);
            ListServers listServers = JsonUtility.FromJson<ListServers>("{\"serverList\":" + json + "}");

            Debug.Log("Parsed Server List: " + listServers.serverList.Length + " servers found.");
            
            foreach (Server server in listServers.serverList) {
                Debug.Log($"Server IP: {server.ip}, Port: {server.port}, Status: {server.status}, Deleted: {server.deleted}");

                if (server.status == ServerStatus.ONLINE.ToString() || server.status == ServerStatus.ALLOCATED.ToString()) {
                    Debug.Log("Found an Online or Allocated server, displaying in UI.");
                    Transform serverTransform = Instantiate(serverTemplate, serverContainer);
                    serverTransform.gameObject.SetActive(true);
                    serverTransform.GetComponent<ServerBrowserSingleUI>().SetServer(
                        server.ip,
                        (ushort)server.port
                    );
                }
            }
        });
    }

#endif

    public class TokenExchangeResponse {
        public string accessToken;
    }


    [Serializable]
    public class TokenExchangeRequest {
        public string[] scopes;
    }

    [Serializable]
    public class QueueAllocationRequest {
        public string allocationId;
        public int buildConfigurationId;
        public string payload;
        public string regionId;
        public bool restart;
    }


    private enum ServerStatus {
        AVAILABLE,
        ONLINE,
        ALLOCATED
    }

    [Serializable]
    public class ListServers {
        public Server[] serverList;
    }

    [Serializable]
    public class Server {
        public int buildConfigurationID;
        public string buildConfigurationName;
        public string buildName;
        public bool deleted;
        public string fleetID;
        public string fleetName;
        public string hardwareType;
        public int id;
        public string ip;
        public int locationID;
        public string locationName;
        public int machineID;
        public int port;
        public string status;
    }

}
}
