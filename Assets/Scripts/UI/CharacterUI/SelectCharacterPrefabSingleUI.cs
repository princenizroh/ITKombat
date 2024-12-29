using UnityEngine;
using UnityEngine.UI;
using System;

namespace ITKombat
{
    public class SelectCharacterPrefabSingleUI : MonoBehaviour
    {
        [SerializeField] private int prefabId;
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectedGameObject;
        [SerializeField] private SelectCharacterInfoBox chararacterInfoBox;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Change player prefab");
                GameMultiplayerManager.Instance.ChangePlayerPrefab(prefabId);
                Debug.Log("Change player prefab done");
                var characterInfo = GameMultiplayerManager.Instance.GetUpdateCharacterInfo(prefabId);
                if (characterInfo != null)
                {
                    chararacterInfoBox.UpdateCharacterInfo(characterInfo);
                }
                else
                {
                    chararacterInfoBox.Hide();
                }

            });
        }
        
        private void Start()
        {
            chararacterInfoBox.Hide();
            GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged += GameMultiplayerManager_OnPlayerDataNetworkListChanged;
            GameMultiplayerManager.Instance.OnDataInitialized += GameMultiplayerManager_OnDataInitialized;
            if (GameMultiplayerManager.Instance.characterStatDictionary != null) {
                InitializeUI();
            }
            
        }
        private void GameMultiplayerManager_OnDataInitialized(object sender, EventArgs e) {
            InitializeUI();
        }

        private void InitializeUI() {
            image.sprite = GameMultiplayerManager.Instance.GetPlayerPrefab(prefabId).GetComponentInChildren<SpriteRenderer>().sprite;
            UpdateIsSelected();
        }
        private void GameMultiplayerManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
            UpdateIsSelected();
            Debug.Log("UpdateIsSelected Called");
        }
        private void UpdateIsSelected()
        {
            Debug.Log("UpdateIsSelected Called 2");
            Debug.Log("PrefabId: " + prefabId);
            Debug.Log("PlayerDataPrefabId: " + GameMultiplayerManager.Instance.GetPlayerData().prefabId);
            if (GameMultiplayerManager.Instance.GetPlayerData().prefabId == prefabId)
            {
                Debug.Log("SelectedGameObject Active");
                selectedGameObject.SetActive(true);
            }
            else
            {
                Debug.Log("SelectedGameObject Inactive");
                selectedGameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayerManager_OnPlayerDataNetworkListChanged;
        }
    }
}
