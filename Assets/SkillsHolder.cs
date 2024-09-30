using NUnit.Framework.Constraints;
using UnityEngine;

namespace ITKombat
{
    public class SkillsHolder : MonoBehaviour
    {
        public Skills skills;
        float cooldownTime;
        float activeTime;

        enum SkillState
        {
            ready,
            active,
            cooldown
        }

        SkillState state = SkillState.ready;

        public KeyCode key; 

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case SkillState.ready:
                    if (Input.GetKey(key))
                    {
                        skills.Activate(gameObject);
                        state = SkillState.active;
                        activeTime = skills.activeTime;
                    }
                break;
                case SkillState.active:
                    if (activeTime > 0)
                    {
                        activeTime -= Time.deltaTime;
                    }
                    else
                    {
                        skills.BeginCooldown(gameObject);
                        state = SkillState.cooldown;
                        cooldownTime = skills.cooldownTime;
                    }
                break;
                case SkillState.cooldown:
                    if (cooldownTime > 0)
                    {
                        cooldownTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = SkillState.ready;
                    }
                    break;
            }
        }
    }
}
