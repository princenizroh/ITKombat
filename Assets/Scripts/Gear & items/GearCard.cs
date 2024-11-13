using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ITKombat
{
    public class GearCard : MonoBehaviour
    {
        // Main
        private GameObject gearMenu;
        private GearManagement gearManagement;

        // Default Properties
        public Sprite AtkIconId;
        public Sprite IntIconId;
        public Sprite DefIconId;
        public Sprite gearTypeIdSprite1;
        public Sprite gearTypeIdSprite2;
        public Sprite gearTypeIdSprite3;

        // Card Properties
        public Image cardBackground;

        // Card Star
        public GameObject star1,star2,star3,star4;

        // Gear
        public Image gearImage;
        public TMP_Text gearNameTMP;
        public Image gearTypeImage;

        // Gear Stats Properties
        public TMP_Text gear1ValueTMP;
        public Image gearStatIcon1;
        public TMP_Text gear2ValueTMP;
        public Image gearStatIcon2;
        public TMP_Text gearLevelTMP;

        // Private Variable
        private int gearId;
        private string gearName;
        private int gearLevel;
        private int gearStat1Id;
        private int gearStat2Id;
        private int gearStat1Value;
        private int gearStat2Value;
        private int ascendLevel;
        private int gearTypeId;
        private bool elitedCard = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            gearMenu = GameObject.Find("GearMenu");
            gearManagement = gearMenu.GetComponent<GearManagement>();    
        }

        // Update is called once per frame
        void Update()
        {

            gearId = gearManagement.gearId;
            gearTypeId = gearManagement.gearTypeId;
            gearStat1Id = gearManagement.gearStat1Id;
            gearStat2Id = gearManagement.gearStat2Id;
            ascendLevel = gearManagement.ascendLevel;
            gearLevel = gearManagement.gearLevel;
            elitedCard = true;

            gearStat1Value = gearManagement.gearStat1Value;
            gearStat2Value = gearManagement.gearStat2Value;

            GearStat[] gearDataArray = Resources.LoadAll<GearStat>("GearData");

            if (gearDataArray.Length == 0)
            {
                Debug.LogWarning("No gear data found in Resources/GearData");
                return;
            }

            foreach (GearStat gearData in gearDataArray) {
                if (gearData.gear_id == gearId) {
                    gearName = gearData.gear_name;
                    gearImage.sprite = gearData.gear_sprite;
                }
            }


            gear1ValueTMP.text = gearStat1Value.ToString();
            gear2ValueTMP.text = gearStat2Value.ToString();
            
            gearNameTMP.text = gearName;
            gearLevelTMP.text = gearLevel.ToString();
            
            // Activate stars based on ascend level
            GameObject[] stars = { star1, star2, star3, star4 };
            for (int i = 0; i < stars.Length; i++) {
                stars[i].SetActive(i < ascendLevel || (ascendLevel == 3 && elitedCard && i == 3));
            }

            // Map gear stat IDs to corresponding icons
            Dictionary<int, Sprite> statIconMapping = new Dictionary<int, Sprite> {
                { 1, AtkIconId },
                { 2, IntIconId },
                { 3, DefIconId }
            };

            if (statIconMapping.TryGetValue(gearStat1Id, out Sprite icon1)) {
                gearStatIcon1.sprite = icon1;
            }
            if (statIconMapping.TryGetValue(gearStat2Id, out Sprite icon2)) {
                gearStatIcon2.sprite = icon2;
            }

            // Map gear type IDs to corresponding sprites
            Dictionary<int, Sprite> typeIconMapping = new Dictionary<int, Sprite> {
                { 1, gearTypeIdSprite1 },
                { 2, gearTypeIdSprite2 },
                { 3, gearTypeIdSprite3 }
            };

            if (typeIconMapping.TryGetValue(gearTypeId, out Sprite typeIcon)) {
                gearTypeImage.sprite = typeIcon;
            }


        }

        void Awake() 
        {

        }
    }
}
