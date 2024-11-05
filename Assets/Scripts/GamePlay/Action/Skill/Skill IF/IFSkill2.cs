using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "IF_Skill2", menuName = "Skills/IF/IF_Skill2", order = 2)]
    public class IFSkill2 : Skills
    {
        private int attacksBlocked = 0;
        public int maxBlocks = 2;

        // Masukin sound dan anim disini

        public override void Activate(GameObject parent)
        {
            attacksBlocked = 0;
            Debug.Log("Skill 1 Aktif");
        }

        public override void BeginCooldown(GameObject parent)
        {
            Debug.Log("Skill 1 Cooldown");
        }
    }
}
