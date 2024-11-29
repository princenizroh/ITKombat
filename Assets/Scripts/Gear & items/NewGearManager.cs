// using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

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
        private PlayerScriptableObject playerScriptableObject;
        private GameFirebase gameFirebase;

        // Inventory Data Ref
        private InventoryListAll[] inventoryDataListAll;
        private InventoryListAll[] inventoryGearTypeData_Head;
        private InventoryListAll[] inventoryGearTypeData_Body;
        private InventoryListAll[] inventoryGearTypeData_Acc;
        
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

        // Basic Function

        void updateInventoryData() {

        }

        void UpdateInventoryItemsPrefab() {
            
        }

    }
}
