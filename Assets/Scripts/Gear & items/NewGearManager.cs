// using Microsoft.Unity.VisualStudio.Editor;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework.Constraints;
using System.Linq;

namespace ITKombat
{
    public class NewGearManager : MonoBehaviour
    {
        // Main Data Ref

        public class InventoryListAll {
            public int item_id_pos;
            public int item_id;
            public int item_type_id;
            public int item_level;
            public int item_ascend;
            public int item_exp_max;
            public int item_current_exp;
            public int item_id_type_1;
            public int item_id_type_2;
            public int item_value_type_1;
            public int item_value_type_2;
        }

        public class ConsumableListAll {
            public int consumableId;
            public int consumableValue;
            public int consumableQuantity;
        }

        private int consumableId;

        // Assign atk data

        public GameObject logoAtk;
        public GameObject LabelAtk;
        public GameObject AtkStat;
        public TMP_Text TMP_AtkStat;
        public GameObject BackgroundAtk;

        public GameObject logoDef;
        public GameObject LabelDef;
        public GameObject DefStat;
        public TMP_Text TMP_DefStat;
        public GameObject BackgroundDef;

        public GameObject logoInt;
        public GameObject LabelInt;
        public GameObject IntStat;
        public TMP_Text TMP_IntStat;
        public GameObject BackgroundInt;

        // Data Ref
        public PlayerScriptableObject playerScriptableObject;
        private GameFirebase gameFirebase;

        // Inventory Data Ref
        private InventoryListAll[] inventoryDataListAll;
        private ConsumableListAll[] consumableDataListAll;
        private List<ConsumableListAll> consumableListAlls = new List<ConsumableListAll>();
        private List<InventoryListAll> inventoryGearTypeData_Head = new List<InventoryListAll>();
        private List<InventoryListAll> inventoryGearTypeData_Body = new List<InventoryListAll>();
        private List<InventoryListAll> inventoryGearTypeData_Acc = new List<InventoryListAll>();
        private List<GameObject> spawnedPrefabs = new List<GameObject>();
        
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
        private int gearIdPos;
        private int gearId;
        private int geartypeid;
        private int gearName;
        private int gearExpMaximum;
        private int currentGearExp;
        private int gearLevel;
        private int gearAscendLevel;

        // Value Ref -> UnityUI
        public TMP_Text TMP_gearName;
        public TMP_Text TMP_gearTypeName;
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

        // Consumable Prefab

        public GameObject prefabDataConsumable;
        public GameObject prefabSpawnLocationConsumable;

        // Consumable Data

        private int consumables_id;
        private int consumables_quantity;
        private int consumables_value;

        // Gear Upgrade Menu

        public GameObject gearUpgradeMenu;
        public TMP_Text gearUpgradeLevel_Text;
        public TMP_Text gearUpgradeExp_Text;
        public TMP_Text gearUpgradeAscend_Text;

        // Upgrade Information
        public GameObject gearUpgradeInformation;
        public TMP_Text gearUpgradeStatus;

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

        public void ClearAllPrefabsAndInventory() {
            // Destroy all spawned prefabs
            foreach (var spawnedPrefab in spawnedPrefabs) {
                if (spawnedPrefab != null) {
                    Destroy(spawnedPrefab);
                }
            }
            
            // Clear the list of prefabs
            spawnedPrefabs.Clear();
            
            // Clear inventory data lists
            inventoryGearTypeData_Head.Clear();
            inventoryGearTypeData_Body.Clear();
            inventoryGearTypeData_Acc.Clear();
            
            Debug.Log("All prefabs and inventory data have been cleared.");
        }

        public async void UpdateGearHead() {
            ClearAllPrefabsAndInventory();
            await updateInventoryDataAsync("atk");
            showInventoryData(inventoryGearTypeData_Head);
        }

        public async void UpdateGearBody() {
            ClearAllPrefabsAndInventory();
            await updateInventoryDataAsync("def");
            showInventoryData(inventoryGearTypeData_Body);
        }

        public async void UpdateGearAcc() {
            ClearAllPrefabsAndInventory();
            await updateInventoryDataAsync("acc");
            showInventoryData(inventoryGearTypeData_Acc);
        }

        // Basic Function

        async Task updateInventoryDataAsync(string type) {

            List<GameFirebase.InventoryItem> playerDataList = await gameFirebase.GetAllPlayerInventory(playerScriptableObject.player_id);

            if (type == "atk") {

                foreach (var playerData in playerDataList) {
                    
                    InventoryListAll appendItem = new InventoryListAll {
                        item_id_pos = playerData.item_id_pos,
                        item_id = playerData.item_id,
                        item_type_id = playerData.item_type_id,
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
                        item_id_pos = playerData.item_id_pos,
                        item_id = playerData.item_id,
                        item_type_id = playerData.item_type_id,
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
                        item_id_pos = playerData.item_id_pos,
                        item_id = playerData.item_id,
                        item_type_id = playerData.item_type_id,
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

        async Task getPlayerConsumableData() {
            consumableListAlls.Clear(); // Clear the list to avoid duplicate data

            List<GameFirebase.ConsumableItem> consumableData = await gameFirebase.GetAllPlayerConsumable(playerScriptableObject.player_id);

            foreach (var playerData in consumableData) {
                ConsumableListAll appendItem = new ConsumableListAll {
                    consumableId = playerData.consumableId,
                    consumableValue = playerData.consumableValue,
                    consumableQuantity = playerData.consumableQuantity
                };

                consumableListAlls.Add(appendItem);
            }

            Debug.Log($"Fetched {consumableListAlls.Count} consumable items.");
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

            // Track the spawned prefab
            spawnedPrefabs.Add(spawnedPrefab);

            // Attach the data to the prefab
            var itemButton = spawnedPrefab.AddComponent<InventoryItemButton>();
            itemButton.itemData = inventData;

            // Add button click event
            Button button = spawnedPrefab.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnInventoryItemClicked(itemButton));
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

        void OnInventoryItemClicked(InventoryItemButton itemButton)
        {
            if (itemButton != null && itemButton.itemData != null)
            {
                GearStat[] gearDataArray = Resources.LoadAll<GearStat>("GearData");
                foreach (GearStat gearData in gearDataArray)
                {
                    if (gearData.gear_id == itemButton.itemData.item_id)
                    {
                        TMP_gearName.text = gearData.gear_name;
                        TMP_gearDesc.text = gearData.gear_desc;
                    }
                }

                Debug.Log("item_id_pos "+ itemButton.itemData.item_id_pos);

                gearIdPos = itemButton.itemData.item_id_pos;
                gearId = itemButton.itemData.item_id;
                geartypeid = itemButton.itemData.item_type_id;
                gearLevel = itemButton.itemData.item_level;
                gearExpMaximum = itemButton.itemData.item_exp_max;
                currentGearExp = itemButton.itemData.item_current_exp;
                gearLevel = itemButton.itemData.item_level;
                gearAscendLevel = itemButton.itemData.item_ascend;

                if (gearAscendLevel == 1) {

                    TMP_gearTypeName.text = "Equipped";

                }

                // mainstatid = 
                // mainstatValue = 
                Debug.Log("type="+itemButton.itemData.item_id_type_1);
                stat1id = itemButton.itemData.item_id_type_1;
                stat1value = itemButton.itemData.item_value_type_1;
                stat2id = itemButton.itemData.item_id_type_2;
                stat2value = itemButton.itemData.item_value_type_2;

                Debug.Log("stid1 "+stat1id);
                Debug.Log("stid2     "+stat2id);

                if (stat1id.ToString() == "1") {
                    Debug.Log("atk here");
                    TMP_AtkStat.text = stat1value.ToString();

                    logoAtk.SetActive(true);
                    LabelAtk.SetActive(true);
                    AtkStat.SetActive(true);
                    BackgroundAtk.SetActive(true);

                    logoDef.SetActive(false);
                    LabelDef.SetActive(false);
                    DefStat.SetActive(false);
                    BackgroundDef.SetActive(false);

                    logoInt.SetActive(false);
                    LabelInt.SetActive(false);
                    IntStat.SetActive(false);
                    BackgroundInt.SetActive(false);
                } else if (stat1id.ToString() == "2") {
                    TMP_DefStat.text = stat1value.ToString();

                    logoAtk.SetActive(false);
                    LabelAtk.SetActive(false);
                    AtkStat.SetActive(false);
                    BackgroundAtk.SetActive(false);

                    logoDef.SetActive(true);
                    LabelDef.SetActive(true);
                    DefStat.SetActive(true);
                    BackgroundDef.SetActive(true);

                    logoInt.SetActive(false);
                    LabelInt.SetActive(false);
                    IntStat.SetActive(false);
                    BackgroundInt.SetActive(false);
                } else {
                    TMP_IntStat.text = stat1value.ToString();

                    logoAtk.SetActive(false);
                    LabelAtk.SetActive(false);
                    AtkStat.SetActive(false);
                    BackgroundAtk.SetActive(false);

                    logoInt.SetActive(true);
                    LabelInt.SetActive(true);
                    IntStat.SetActive(true);
                    BackgroundInt.SetActive(true);

                    logoDef.SetActive(false);
                    LabelDef.SetActive(false);
                    DefStat.SetActive(false);
                    BackgroundDef.SetActive(false);
                }

                if (stat2id.ToString() == "1") {
                    TMP_AtkStat.text = stat2value.ToString();

                    logoAtk.SetActive(true);
                    LabelAtk.SetActive(true);
                    AtkStat.SetActive(true);
                    BackgroundAtk.SetActive(true);

                } else if (stat2id.ToString() == "2") {
                    TMP_DefStat.text = stat2value.ToString();

                    logoDef.SetActive(true);
                    LabelDef.SetActive(true);
                    DefStat.SetActive(true);
                    BackgroundDef.SetActive(true);

                } else {
                    TMP_IntStat.text = stat2value.ToString();

                    logoInt.SetActive(true);
                    LabelInt.SetActive(true);
                    IntStat.SetActive(true);
                    BackgroundInt.SetActive(true);
                }

                TMP_gearLevel.text = "+" + itemButton.itemData.item_level;
                TMP_mainstatValue.text = "Main Stat Value: " + itemButton.itemData.item_value_type_1;
            }
            else
            {
                Debug.LogError("Item button or its data is null!");
            }
        }

        public async void UpdateSelectedGear(int gearId, string consumable_id)
        {
            Task<List<GameFirebase.ConsumableItem>> consumableDataTask = gameFirebase.GetPlayerConsumableData(playerScriptableObject.player_id, consumable_id);
            List<GameFirebase.ConsumableItem> consumableDataList = await consumableDataTask;

            GameFirebase.ConsumableItem selectedConsumable = consumableDataList.FirstOrDefault(item => item.consumableId.ToString() == consumable_id);

            if (selectedConsumable != null)
            {
                if (selectedConsumable.consumableQuantity > 0)
                {
                    var calculated_gear_upgrade = currentGearExp + selectedConsumable.consumableValue;
                    gameFirebase.EditPlayerConsumableData(playerScriptableObject.player_id,consumable_id,-1);
                    if (calculated_gear_upgrade > gearExpMaximum) {
                        gameFirebase.EditPlayerGearData(playerScriptableObject.player_id,gearId,"item_level",gearLevel+1);
                    }
                }
                else
                {
                    Debug.LogWarning($"Consumable {consumable_id} quantity is zero.");
                }
            }
            else
            {
                Debug.LogWarning($"Consumable with ID {consumable_id} not found.");
            }
        }

        public void gearUpgradeButtonPressed() {

            if (gearAscendLevel == 0) {
                
                StartCoroutine(gameFirebase.ChangeValueIntegerForGear(playerScriptableObject.player_id, "inventory", gearIdPos.ToString(), "item_ascend",1));

            } else {

                StartCoroutine(gameFirebase.ChangeValueIntegerForGear(playerScriptableObject.player_id, "inventory", gearIdPos.ToString(), "item_ascend",0));
                TMP_gearTypeName.text = "Not Equipped";

            }


            // gearUpgradeMenu.SetActive(true);

            // // Display initial upgrade info
            // gearUpgradeAscend_Text.text = gearAscendLevel.ToString();
            // gearUpgradeLevel_Text.text = gearLevel.ToString();
            // gearUpgradeExp_Text.text = currentGearExp.ToString() + "/" + gearExpMaximum.ToString();

            // // Clear previous consumable data
            // consumableListAlls.Clear();

            // // Fetch and then display data
            // await getPlayerConsumableData(); // Ensure the data is fetched before proceeding
            // showConsumableData(consumableListAlls); // Display data after it's ready
        }


        public void closeGearUpgradeMenu() {
            gearUpgradeMenu.SetActive(false);
        }

        void showConsumableData(List<ConsumableListAll> consumableData) {
            if (consumableData == null) {
                Debug.LogError("consumableData list is null!");
                return;
            }

            Debug.Log($"Now spawning inventory data. List count: {consumableData.Count}");
            foreach (var spawnConsumableData in consumableData) {
                if (spawnConsumableData == null) {
                    Debug.LogError("spawnInventData is null!");
                    continue;
                }
                // Log detailed information for each item
                Debug.Log($"Consumable Id: {spawnConsumableData.consumableId}, " +
                        $"Consumable Quantity: {spawnConsumableData.consumableQuantity}, " +
                        $"Consumable Value: {spawnConsumableData.consumableValue}");
                spawnPrefabConsumable(spawnConsumableData);
            }
        }

        void spawnPrefabConsumable(ConsumableListAll consumableData)
        {
            Debug.Log("data retrieved: " + consumableData.consumableId);

            GameObject spawnedPrefab = Instantiate(prefabDataConsumable);
            spawnedPrefab.transform.SetParent(prefabSpawnLocationConsumable.transform);
            spawnedPrefab.transform.localPosition = Vector3.zero;
            spawnedPrefab.transform.localRotation = Quaternion.identity;
            spawnedPrefab.transform.localScale = Vector3.one;

            // Track the spawned prefab
            spawnedPrefabs.Add(spawnedPrefab);

            // Attach the data to the prefab
            var itemButton = spawnedPrefab.AddComponent<ConsumableItemButton>();
            itemButton.consumableData = consumableData;

            // Add button click event
            Button button = spawnedPrefab.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnConsumableItemClicked(itemButton));
            }

            // Change data inside the prefab (e.g., image, text)
            Image prefabImage = spawnedPrefab.GetComponentInChildren<Image>();
            TextMeshProUGUI prefabLevel = spawnedPrefab.GetComponentInChildren<TextMeshProUGUI>();

            prefabLevel.text = consumableData.consumableValue.ToString();

            // // Set the sprite
            // GearStat[] gearDataArray = Resources.LoadAll<GearStat>("GearData");
            // foreach (GearStat gearData in gearDataArray)
            // {
            //     if (gearData.gear_id == consumableData.item_id)
            //     {
            //         prefabImage.sprite = gearData.gear_sprite;
            //     }
            // }
        }

        void OnConsumableItemClicked(ConsumableItemButton itemButton)
        {
            if (itemButton != null && itemButton.consumableData != null)
            {

                consumables_id = itemButton.consumableData.consumableId;
                consumables_quantity = itemButton.consumableData.consumableQuantity;
                consumables_value = itemButton.consumableData.consumableValue;

            }
            else
            {
                Debug.LogError("Item button or its data is null!");
            }
        }
    }
}
