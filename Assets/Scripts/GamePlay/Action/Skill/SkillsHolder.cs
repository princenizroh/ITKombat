using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    public class SkillsHolder : MonoBehaviour
    {
        public Skills[] currentSkills;

        private SkillState[] states;
        float[] cooldownTime;
        float[] activeTime;

        private Animator anim;

        enum SkillState
        {
            ready,
            active,
            cooldown
        }

        private void Start()
        {
            anim = GetComponent<Animator>();
            cooldownTime = new float[currentSkills.Length];
            activeTime = new float[currentSkills.Length];
            states = new SkillState[currentSkills.Length];

        for (int i = 0; i < currentSkills.Length; i++)
            {
                states[i] = SkillState.ready;
            }
        }

        public void ActivateSkill(int index)
        {
            if (currentSkills == null || index < 0 || index >= currentSkills.Length)
            {
                Debug.LogError("Invalid skill index or no skills assigned.");
                return;
            }

            if (states[index] == SkillState.ready)
            {
                currentSkills[index].Activate(gameObject); // Call the Activate method of the skill

                // Trigger animation
                if (anim != null)
                {
                    anim.SetTrigger("skill" + (index + 1));
                }

                states[index] = SkillState.active;
                activeTime[index] = currentSkills[index].activeTime;

                StartCoroutine(ResetToIdleAfterTime(index, currentSkills[index].activeTime));
            }
        }
        
        public void ActivateSkill1()
        {
            ActivateSkill(0);
        }

        public void ActivateSkill2()
        {
            ActivateSkill(1);
        }

        public void ActivateSkill3()
        {
            ActivateSkill(2);
        }

        private IEnumerator ResetToIdleAfterTime(int index, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (anim != null)
            {
                anim.SetTrigger("idle");
            }

            // Reset the state of the skill
            states[index] = SkillState.cooldown;
            cooldownTime[index] = currentSkills[index].cooldownTime;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < currentSkills.Length;i++)
            {
                switch (states[i])
                {
                    case SkillState.active:
                        if (activeTime[i] > 0)
                        {
                            activeTime[i] -= Time.deltaTime;
                        }
                        else
                        {
                            currentSkills[i].BeginCooldown(gameObject);
                            states[i] = SkillState.cooldown;
                            cooldownTime[i] = currentSkills[i].cooldownTime;
                        }
                        break;
                    case SkillState.cooldown:
                        if (cooldownTime[i] > 0)
                        {
                            cooldownTime[i] -= Time.deltaTime;
                        }
                        else
                        {
                            states[i] = SkillState.ready;
                        }
                        break;
                }
            }
        }
    }
}
