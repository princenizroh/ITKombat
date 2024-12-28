
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Fisika_Skill2", menuName = "Skills/Fisika/Fisika_Skill2", order = 2)]
    public class FisikaSkill2 : Skills
    {
        // Masukin sound dan anim disini
        public float attackBuff = 10f;
        public float defenseBuff = 10f;
        public float buffDuration = 5f;

        public override void Activate(GameObject parent)
        {
/*            PlayerStats stats = parent.GetComponent<PlayerStats>();

            // Apply the buff to the player
            stats.attackPower += attackBuff;
            stats.defensePower += defenseBuff;

            // suara skill 2
            // NewSoundManager.Instance.PlaySound("Fisika_Skill2", parent.transform.position);

            Debug.Log("Attack and defense buffed.");*/
        }

        public override void BeginCooldown(GameObject parent)
        {
/*            PlayerStats stats = parent.GetComponent<PlayerStats>();

            // Remove the buff after the duration is over
            stats.attackPower -= attackBuff;
            stats.defensePower -= defenseBuff;

            Debug.Log("Buff deactivated, stats reset.");*/
        }
    }
}
