using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

namespace ITKombat
{
    public class GearManagement : MonoBehaviour
    {
        public int hpStat;
        public int atkStat;
        public int spdStat;
        public int gearLevel;
        public int gearExp;
        public int gearExpReq = 50;

        public void gearUpgrade() {

            if (gearExp >= gearExpReq) {

                // tiap penambahan level dari sebuah gear, maka kebutuhan exp dari gear akan meningkat sebesar 0.5
                int finalExpRequired = gearExpReq * 1/2;
                gearExpReq = finalExpRequired;

                // menambahkan level pada gear
                gearLevel =+ 1;

                if (gearLevel >= 4) {
                    
                } else if (gearLevel >= 8) {
                    
                } else if (gearLevel >= 12) {

                } else if (gearLevel >= 15) {

                }
            }

        }
    }
}
