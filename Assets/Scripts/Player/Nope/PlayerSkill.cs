using UnityEngine;

namespace ITKombat
{
    public class PlayerSkill : MonoBehaviour
    {
        public new string name;
        public float cooldownTime;
        public float activeTime;

        public virtual void Activate(GameObject parent) { }
        public virtual void BeginCooldown(GameObject parent) { }
        
        
    }
}
