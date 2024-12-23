using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class SelectCharacterPrefabSingleUI : MonoBehaviour
    {
        [SerializeField] private int prefabId;
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectedGameObject;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Change player prefab");
                GameMultiplayerManager.Instance.ChangePlayerPrefab(prefabId);
                Debug.Log("Change player prefab done");
            });
        }
        private void Start()
        {
            GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged += GameMultiplayerManager_OnPlayerDataNetworkListChanged;
            Debug.Log("PlayerDataNetworkListChanged");
            image.sprite = GameMultiplayerManager.Instance.GetPlayerPrefab(prefabId).GetComponentInChildren<SpriteRenderer>().sprite;
            Debug.Log("Image Sprite");
            UpdateIsSelected();
            Debug.Log("UpdateIsSelected");
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
