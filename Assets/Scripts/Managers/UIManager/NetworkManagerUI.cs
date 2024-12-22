using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button serverBtn;
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button clientBtn;

        private void Awake()
        {
            hostBtn.onClick.AddListener(() => 
                {
                    GameMultiplayerManager.Instance.StartHost();
                    Loader.LoadNetwork(Loader.Scene.SelectCharacterMultiplayer);
                    Hide();
                });
            clientBtn.onClick.AddListener(() => 
                {
                    GameMultiplayerManager.Instance.StartClient();
                    Hide();
                });
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}
