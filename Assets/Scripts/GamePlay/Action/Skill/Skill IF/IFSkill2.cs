using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "IF_Skill2", menuName = "Skills/IF/IF_Skill2", order = 2)]
    public class IFSkill2 : Skills
    {
        private int attacksBlocked = 0;
        public int maxBlocks = 2;
        private bool skillActive = false;

        // Masukin sound dan anim disini

        public override void Activate(GameObject parent)
        {
            attacksBlocked = 0;
            skillActive = true;
            // NewSoundManager.Instance.PlaySound("IF_Skill2", parent.transform.position);
            // Debug.Log("Skill 2 Aktif");
        }

        public override void BeginCooldown(GameObject parent)
        {
            skillActive = false;
            // Debug.Log("Skill 2 Cooldown");
        }

        public bool BlockAttack() //tambahkan GameObject parent di argument ini
        {
            if (!skillActive) return false;

            if (attacksBlocked < maxBlocks)
            {
                attacksBlocked++;
                Debug.Log("Sisa block: " + attacksBlocked);

                if (attacksBlocked >= maxBlocks)
                {
                    skillActive = false;
                    // NewSoundManager.Instance.PlaySound("IF_Skill2_Pecah", parent.transform.position);
                    Debug.Log("Skill 2 habis");
                }

                return true;
            }

            return false;
        }

        public bool IsActive()
        {
            return skillActive;
        }
    }
}
