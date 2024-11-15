using GooglePlayGames.BasicApi;
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Kelautan_Skill2", menuName = "Skills/Kelautan/Kelautan_Skill2", order = 2)]
    public class KelautanSkill2 : Skills
    {
        // Masukin sound dan anim disini
        public float attackBuff = 15f;
        public float defenseBuff = 15f;
        public float buffDuration = 5f;

        public override void Activate(GameObject parent)
        {
/*            // Masukin sound dan anim disini
            PlayerStats stats = player.GetComponent<PlayerStats>();

            // Apply buffs
            stats.attackPower += attackBuff;
            stats.defensePower += defenseBuff;
            Debug.Log("Attack and defense buffed.");

            // Buff duration timer
            player.GetComponent<PlayerController>().Invoke("RemoveBuffs", buffDuration);*/
        }

        public override void BeginCooldown(GameObject parent)
        {
/*            PlayerStats stats = GetComponent<PlayerStats>();
            stats.attackPower -= attackBuff;
            stats.defensePower -= defenseBuff;
            Debug.Log("Buffs removed.");*/
        }
    }
}
