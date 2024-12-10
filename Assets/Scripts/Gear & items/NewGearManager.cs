// using Microsoft.Unity.VisualStudio.Editor;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class NewGearManager : MonoBehaviour
    {
        // Main Data Ref

        public class InventoryListAll {
            public int item_id;
            public int item_level;
            public int item_ascend;
            public int item_exp_max;
            public int item_current_exp;
            public int item_id_type_1;
            public int item_id_type_2;
            public int item_value_type_1;
            public int item_value_type_2;
        }

        // Data Ref
        public PlayerScriptableObject playerScriptableObject;
        private GameFirebase gameFirebase;

        // Inventory Data Ref
        private InventoryListAll[] inventoryDataListAll;
        private List<InventoryListAll> inventoryGearTypeData_Head = new List<InventoryListAll>();
        private List<InventoryListAll> inventoryGearTypeData_Body = new List<InventoryListAll>();
        private List<InventoryListAll> inventoryGearTypeData_Acc = new List<InventoryListAll>();
        
        // Sprite Ref
        public Sprite attackSprite;
        public Sprite defenseSprite;
        public Sprite intSprite;

        // Value Ref
        private int mainstatid;
        private int mainstatValue;
        private int stat1id;
        private int stat1value;
        private int stat2id;
        private int stat2value;

        // Card Detail Ref
        private int gearId;
        private int gearName;
        private int gearExpMaximum;
        private int currentGearExp;
        private int gearLevel;
        private int gearAscendLevel;

        // Value Ref -> UnityUI
        public TMP_Text TMP_gearName;
        public TMP_Text TMP_gearClass;
        public TMP_Text TMP_gearLevel;
        public TMP_Text TMP_gearDesc;
        public TMP_Text TMP_mainstatcodename;
        public TMP_Text TMP_mainstatValue;
        public TMP_Text TMP_stat1codename;
        public TMP_Text TMP_stat1Value;
        public TMP_Text TMP_stat2codename;
        public TMP_Text TMP_stat2Value;

        // Prefab Spawn

        public GameObject prefabData;
        public GameObject prefabSpawnLocation;

        // Image Value Ref -> UnityUI
        // public Image mainStat;
        // public Image stat1;
        // public Image stat2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            gameFirebase = GameFirebase.instance;
        }

        // Update is called once per frame
        void Update() {
            
        }

        // Button

        void AscendGear() {

        }

        void UpgradeGear() {

        }

        public async void UpdateGearHead() {
            await updateInventoryDataAsync("atk");
            showInventoryData(inventoryGearTypeData_Head);
        }

        // Basic Function

        async Task updateInventoryDataAsync(string type) {

            List<GameFirebase.InventoryItem> playerDataList = await gameFirebase.GetAllPlayerInventory(playerScriptableObject.player_id);

            if (type == "atk") {

                foreach (var playerData in playerDataList) {
                    
                    InventoryListAll appendItem = new InventoryListAll {
                        item_id = playerData.item_id,
                        item_level = playerData.item_level,
                        item_ascend = playerData.item_ascend,
                        item_exp_max = playerData.item_exp_max,
                        item_current_exp = playerData.item_current_exp,
                        item_id_type_1 = playerData.item_id_type_1,
                        item_id_type_2 = playerData.item_id_type_2,
                        item_value_type_1 = playerData.item_value_type_1,
                        item_value_type_2 = playerData.item_value_type_2
                    };

                    inventoryGearTypeData_Head.Add(appendItem);

                }

            } else if (type == "def") {

                foreach (var playerData in playerDataList) {
                    
                    InventoryListAll appendItem = new InventoryListAll {
                        item_id = playerData.item_id,
                        item_level = playerData.item_level,
                        item_ascend = playerData.item_ascend,
                        item_exp_max = playerData.item_exp_max,
                        item_current_exp = playerData.item_current_exp,
                        item_id_type_1 = playerData.item_id_type_1,
                        item_id_type_2 = playerData.item_id_type_2,
                        item_value_type_1 = playerData.item_value_type_1,
                        item_value_type_2 = playerData.item_value_type_2
                    };

                    inventoryGearTypeData_Head.Add(appendItem);

                }

            } else if (type == "acc") {

                foreach (var playerData in playerDataList) {
                    
                    InventoryListAll appendItem = new InventoryListAll {
                        item_id = playerData.item_id,
                        item_level = playerData.item_level,
                        item_ascend = playerData.item_ascend,
                        item_exp_max = playerData.item_exp_max,
                        item_current_exp = playerData.item_current_exp,
                        item_id_type_1 = playerData.item_id_type_1,
                        item_id_type_2 = playerData.item_id_type_2,
                        item_value_type_1 = playerData.item_value_type_1,
                        item_value_type_2 = playerData.item_value_type_2
                    };

                    inventoryGearTypeData_Head.Add(appendItem);

                }

            }

        }

        void showInventoryData(List<InventoryListAll> inventoryData) {
            if (inventoryData == null) {
                Debug.LogError("Inventory data list is null!");
                return;
            }

            Debug.Log($"Now spawning inventory data. List count: {inventoryData.Count}");
            foreach (var spawnInventData in inventoryData) {
                if (spawnInventData == null) {
                    Debug.LogError("spawnInventData is null!");
                    continue;
                }
                // Log detailed information for each item
                Debug.Log($"Item ID: {spawnInventData.item_id}, " +
                        $"Level: {spawnInventData.item_level}, " +
                        $"Ascend: {spawnInventData.item_ascend}, " +
                        $"ExpMax: {spawnInventData.item_exp_max}, " +
                        $"CurrentExp: {spawnInventData.item_current_exp}");
                spawnPrefab(spawnInventData);
            }
        }

        void spawnPrefab(InventoryListAll inventData)
        {
            Debug.Log("data retrieved: " + inventData.item_id);

            GameObject spawnedPrefab = Instantiate(prefabData);
            spawnedPrefab.transform.SetParent(prefabSpawnLocation.transform);
            
            spawnedPrefab.transform.localPosition = Vector3.zero;
            spawnedPrefab.transform.localRotation = Quaternion.identity;
            spawnedPrefab.transform.localScale = Vector3.one;

            // Attach the data to the prefab
            var itemButton = spawnedPrefab.AddComponent<InventoryItemButton>();
            itemButton.itemData = inventData;

            // Add button click event
            Button button = spawnedPrefab.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnPrefabButtonClicked(itemButton));
            }

            // Change data inside the prefab (e.g., image, text)
            Image prefabImage = spawnedPrefab.GetComponentInChildren<Image>();
            TextMeshProUGUI prefabLevel = spawnedPrefab.GetComponentInChildren<TextMeshProUGUI>();

            prefabLevel.text = inventData.item_level.ToString();

            // Set the sprite
            GearStat[] gearDataArray = Resources.LoadAll<GearStat>("GearData");
            foreach (GearStat gearData in gearDataArray)
            {
                if (gearData.gear_id == inventData.item_id)
                {
                    prefabImage.sprite = gearData.gear_sprite;
                }
            }
        }

        void OnPrefabButtonClicked(InventoryItemButton itemButton)
        {
            if (itemButton != null && itemButton.itemData != null)
            {
                Debug.Log($"Button clicked! Item ID: {itemButton.itemData.item_id}, Level: {itemButton.itemData.item_level}");
                
                // Perform actions with the data
                // For example, show detailed information about the item
                TMP_gearName.text = "Item ID: " + itemButton.itemData.item_id;
                TMP_gearLevel.text = "Level: " + itemButton.itemData.item_level;
                TMP_mainstatValue.text = "Main Stat Value: " + itemButton.itemData.item_value_type_1;
            }
            else
            {
                Debug.LogError("Item button or its data is null!");
            }
        }



        void UpdateInventoryItemsPrefab() {
            
        }

    }
}
