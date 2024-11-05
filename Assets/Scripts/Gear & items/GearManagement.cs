using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ITKombat.GameFirebase;

namespace ITKombat
{
    [Serializable]
    public class GearStats {
        public int statsCode_1;
        public int statsCode_2;
        public int statsValue_1;
        public int statsValue_2;
    }

    [Serializable]
    public class IntgearStats {
        public string statsName;
        public int statsCode;
        public int statsMultiplier;
        public int statsRange;
    }
    [Serializable]
    public class AtkgearStats {
        public string statsName;
        public int statsCode;
        public int statsMultiplier;
        public int statsRange;
    }
    [Serializable]
    public class DefgearStats {
        public string statsName;
        public int statsCode;
        public int statsMultiplier;
        public int statsRange;
    }
    [Serializable]
    public class IfMaterial {
        public string itemName;
        public int itemQuantitiy;
        public int itemCode;
    }

    public class GearManagement : MonoBehaviour
    {   
        public IntgearStats intGear;
        public AtkgearStats atkGear;
        public DefgearStats defGear;
        public int gearId;
        public GearStats gearStats;
        public TMP_Text gearLevelText;
        public TMP_Text gearExpStatusText;
        public TMP_Text gearMaterialList;
        public TMP_Text gearStats1;
        public TMP_Text gearStats2;
        public GameObject warningInsufficientText;
        public GameObject warningAscendText;
        public GameObject upgradeGearButtonGO;
        public Button upgradeGearButton;
        public Button ascendGearButton;
        public int gearLevel;
        public int gearExpMax;
        public int gearExpStatus;
        private bool gearAscendStatus = false;
        private GameFirebase gameFirebase;
        public PlayerScriptableObject playerData;

        void Start()
        {
            gameFirebase = GameFirebase.instance;
            ascendGearButton.enabled = false;

            StartCoroutine(GetPlayerGearDataCoroutine(playerData.player_id, 21));
        }

        void Update()
        {
            gearLevelText.SetText("Gear Level " + gearLevel + "/15");
            gearExpStatusText.SetText("Gear Exp Status: " + gearExpStatus + "/" + gearExpMax);
            gearStats1.SetText(gearStats.statsCode_1 + ":" + gearStats.statsValue_1);
            gearStats2.SetText(gearStats.statsCode_2 + ":" + gearStats.statsValue_2);
        }

        private IEnumerator GetPlayerGearDataCoroutine(string player_id, int gear_id)
        {
            // SingleDataOnly
            Task<List<InventoryItem>> task = gameFirebase.GetPlayerGearData(player_id, gear_id);
            yield return new WaitUntil(() => task.IsCompleted);

            List<InventoryItem> playerGearData = task.Result;
            foreach (var item in playerGearData)
            {
                gearExpStatus = item.item_current_exp;
                gearExpMax = item.item_exp_max;
                gearLevel = item.item_level;
                gearStats.statsCode_1 = item.item_id_type_1;
                gearStats.statsCode_2 = item.item_id_type_2;
                gearStats.statsValue_1 = item.item_value_type_1;
                gearStats.statsValue_2 = item.item_value_type_2;
            }
        }

        public void upgradeGear(int exp) {

            gearExpStatus += exp;
            StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_current_exp", gearExpStatus));

            if (gearLevel == 15) {

                upgradeGearButtonGO.SetActive(false);
                ascendGearButton.enabled = false;

            } else if (gearLevel == 5 & gearAscendStatus == false) {

                warningAscendText.SetActive(true);
                warningInsufficientText.SetActive(true);
                upgradeGearButton.enabled = false;
                ascendGearButton.enabled = true;

                gearMaterialList.SetText("Plutonium Atom");

            } else if (gearLevel == 10 & gearAscendStatus == false) {

                warningAscendText.SetActive(true);
                warningInsufficientText.SetActive(true);
                upgradeGearButton.enabled = false;
                ascendGearButton.enabled = true;

                gearMaterialList.SetText("Exa Atom");

            } else if (gearLevel == 13 & gearAscendStatus == false) {

                warningAscendText.SetActive(true);
                warningInsufficientText.SetActive(true);
                upgradeGearButton.enabled = false;
                ascendGearButton.enabled = true;

                gearMaterialList.SetText("Val Atom");

            } else if (gearExpStatus >= gearExpMax) {

                int gearCalculation = gearExpStatus - gearExpMax;

                gearLevel += 1;
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_level", gearLevel));

                gearExpMax *= 2;
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_exp_max", gearExpMax));

                gearExpStatus = 0;
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_current_exp", gearExpStatus));

                gearAscendStatus = false;

                if (gearCalculation > 0) {

                    gearExpStatus += gearCalculation;
                    Debug.Log("overflow exp, added into the gearexpstatus");

                }
                
            }

        }

        public void ascendGear() {

            warningAscendText.SetActive(false);
            warningInsufficientText.SetActive(false);
            upgradeGearButton.enabled = true;
            gearAscendStatus = true;

            object[] gearStatArray = {
                intGear, atkGear, defGear
            };

            int randomPickGear = UnityEngine.Random.Range(0, gearStatArray.Length);
            object pickedRandomStat = gearStatArray[randomPickGear];
            
            if (pickedRandomStat is IntgearStats intStat) {
                AscendStat(1, intStat.statsCode, ref gearStats.statsCode_1, ref gearStats.statsValue_1, ref gearStats.statsCode_2, ref gearStats.statsValue_2);
            } else if (pickedRandomStat is AtkgearStats atkStat) {
                AscendStat(2, atkStat.statsCode, ref gearStats.statsCode_1, ref gearStats.statsValue_1, ref gearStats.statsCode_2, ref gearStats.statsValue_2);
            } else if (pickedRandomStat is DefgearStats defStat) {
                AscendStat(3, defStat.statsCode, ref gearStats.statsCode_1, ref gearStats.statsValue_1, ref gearStats.statsCode_2, ref gearStats.statsValue_2);
            }

            if (gearLevel > 13) {

                upgradeGearButtonGO.SetActive(false);

            }

        }

        public void resetGear() {

            gearStats.statsCode_1 = 0;
            gearStats.statsCode_2 = 0;
            gearStats.statsValue_1 = 0;
            gearStats.statsValue_2 = 0;
            gearExpMax = 100;
            gearExpStatus = 0;
            gearLevel = 0;

        }

        void AscendStat(int newCode, int statCode, ref int code1, ref int value1, ref int code2, ref int value2) {
            if (code1 == statCode) {
                value1 *= 2;
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_value_type_1", value1));
                Debug.Log("Succesfully upgraded stat 1");
            } else if (code2 == statCode) {
                value2 *= 2;
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_value_type_2", value2));
                Debug.Log("Succesfully upgraded stat 2");
            } else if (code1 == 0) {
                code1 = newCode;
                value1 = UnityEngine.Random.Range(10, 21);
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_id_type_1", code1));
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_value_type_1", value1));
                Debug.Log("Added new stat 1: code " + code1 + ", value " + value1);
            } else if (code2 == 0) {
                code2 = newCode;
                value2 = UnityEngine.Random.Range(10, 21);
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_id_type_2", code2));
                StartCoroutine(gameFirebase.EditPlayerGearData(playerData.player_id, 21, "item_value_type_2", value2));
                Debug.Log("Added new stat 2: code " + code2 + ", value " + value2);
            } else {
                Debug.Log("All stats are filled.");
            }
        }

    }
}
