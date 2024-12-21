using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace ITKombat
{
    public class ServerBrowserSingleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI ipText; 
        [SerializeField] private TextMeshProUGUI portText;

        private void Awake() {
            Button button = GetComponent<Button>();
            if (button != null) {
                button.onClick.AddListener(() => {
                    if (ipText != null && portText != null) {
                        string ipv4Address = ipText.text;
                        ushort port = ushort.Parse(portText.text);
                        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

                        GameMultiplayerManager.Instance.StartClient();
                    } else {
                        Debug.LogError("ipText or portText is not set.");
                    }
                });
            } else {
                Debug.LogError("Button component is not found.");
            }
        }

        public void SetServer(string ip, ushort port) {
            if (ipText != null && portText != null) {
                ipText.text = ip;
                portText.text = port.ToString();
            } else {
                Debug.LogError("ipText or portText is not set.");
            }
        }
    }
}

