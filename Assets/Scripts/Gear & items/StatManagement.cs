using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    public class StatManagement : MonoBehaviour
    {
        public int playerHealth;
        public int playerHealthCap;
        public int characterBaseDefense;
        public int characterBaseInt;

        void Start() {
            
            // characterBaseDefense = character.characterBaseDefense;

        }
        
        public void playerRecieveDamage(int raw_damage)
        {     
            float evasionChance = Random.Range(0f, 1f);

            float calculation = evasionChance / 160;

            if (calculation >= 0.009) {

                playerCommitEvasion(characterBaseInt, raw_damage);
            
            } else {

                playerHealth -= raw_damage;

            }
        }

        public void playerRecieveHealth(int raw_health) {

        }

        public void playerCommitEvasion(int raw_int, int raw_damage) {

            int baseChance = 5;
            float successChance = 0.010f;
            int calculation = (raw_damage - raw_int) / baseChance * (raw_int/raw_damage);

            if (calculation >= successChance) {
                
                playerHealth += 0;

            }

        }

    }
}
