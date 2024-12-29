
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Kimia_Skill3", menuName = "Skills/Kimia/Kimia_Skill3", order = 3)]
    public class KimiaSkill3 : Skills
    {
        public float attackBoostMultiplier = 1.5f;
        public float defenseBoostMultiplier = 1.5f;

        public override void Activate(GameObject parent)
        {
/*            var playerStats = parent.GetComponent<PlayerStats>(); // Assuming there's a PlayerStats script
            playerStats.attackPower *= attackBoostMultiplier;
            playerStats.defensePower *= defenseBoostMultiplier;

            // suara skill 3 
            // NewSoundManager.Instance.PlaySound("Kimia_Skill3", parent.transform.position);
            Debug.Log("Attack and defense boosted.")*/;
        }

        public override void BeginCooldown(GameObject parent)
        {
/*            var playerStats = parent.GetComponent<PlayerStats>();
            playerStats.attackPower /= attackBoostMultiplier;
            playerStats.defensePower /= defenseBoostMultiplier;
            Debug.Log("Boost ended, attack and defense returned to normal.");*/
        }
    }
}
