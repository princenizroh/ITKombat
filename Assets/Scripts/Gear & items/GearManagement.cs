using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

namespace ITKombat
{
    public class GearManagement : MonoBehaviour
    {
        
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
        public int gearExpMax = 100;
        public int gearExpStatus;
        private bool gearAscendStatus = false;

        public void upgradeGear(int exp) {

            int gearExpStatusFixed = gearExpMax - gearExpStatus;

            if (gearExpStatusFixed < 0) {

                int expstatuschange = gearExpStatusFixed * -1;

                gearExpStatus =+ expstatuschange;

            } else {

                gearExpStatus += exp;

                Debug.Log("added " + exp + " exps to the gear");

            }

            if (gearLevel == 15) {

                upgradeGearButtonGO.SetActive(false);

            } else if (gearLevel == 5 & gearAscendStatus == false) {

                warningAscendText.SetActive(true);
                upgradeGearButton.enabled = false;

            } else if (gearLevel == 10 & gearAscendStatus == false) {

                warningAscendText.SetActive(true);
                upgradeGearButton.enabled = false;

            } else if (gearLevel == 13 & gearAscendStatus == false) {

                warningAscendText.SetActive(true);
                upgradeGearButton.enabled = false;

            } else if (gearExpStatus >= gearExpMax) {

                gearLevel += 1;
                gearExpMax *= 2;

                Debug.Log("gear upgraded");
                gearAscendStatus = false;
                
            }

        }

        public void ascendGear() {

            warningAscendText.SetActive(false);
            upgradeGearButton.enabled = true;
            gearAscendStatus = true;

            if (gearLevel > 13) {

                upgradeGearButtonGO.SetActive(false);

            }

        }

    }
}
