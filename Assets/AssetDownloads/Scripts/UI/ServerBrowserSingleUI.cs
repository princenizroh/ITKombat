using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowserSingleUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI ipText; 
    [SerializeField] private TextMeshProUGUI portText;


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            string ipv4Address = ipText.text;
            ushort port = ushort.Parse(portText.text);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

            KitchenGameMultiplayer.Instance.StartClient();
        });
    }

    public void SetServer(string ip, ushort port) {
        ipText.text = ip;
        portText.text = port.ToString();
    }

}