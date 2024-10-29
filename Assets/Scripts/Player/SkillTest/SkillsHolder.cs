using NUnit.Framework.Constraints;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITKombat
{
    public class SkillsHolder : MonoBehaviour
    {
        public Skills[] skills;
        public AudioSource[] skillSounds;
        float[] cooldownTime;
        float[] activeTime;

        private InputAction[] skillsAction;
        private SkillState[] states;

        public InputActionAsset inputActionAsset;
        public string[] skillActionNames;
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
            cooldownTime = new float[skills.Length];
            activeTime = new float[skills.Length];
            states = new SkillState[skills.Length];
            skillsAction = new InputAction[skills.Length];

        if (skillSounds.Length != skills.Length)
            {
                Debug.LogError("The number of skill sounds does not match the number of skills");
                return;
            }

        for (int i = 0; i < skills.Length; i++)
            {
                states[i] = SkillState.ready;

                skillsAction[i] = inputActionAsset.FindAction(skillActionNames[i]);
                skillsAction[i].performed += ctx => ActivateSkill(i);
                skillsAction[i].Enable(); // Enable the input action
            }
        }

        public void ActivateSkill(int index)
        {
            if (states[index] == SkillState.ready)
            {
                skills[index].Activate(gameObject);
                states[index] = SkillState.active;
                activeTime[index] = skills[index].activeTime;

                if (anim != null)
                {
                    anim.SetTrigger("skill" + (index + 1));
                }

                if (skillSounds[index] != null)
                {
                    PlaySound(skillSounds[index]);
                }
                StartCoroutine(ResetToIdleAfterTime(index, skills[index].activeTime));
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
            cooldownTime[index] = skills[index].cooldownTime;
        }

        private void PlaySound(AudioSource sound)
        {
            if (sound != null && !sound.isPlaying)
            {
                sound.Play();
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < skills.Length;i++)
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
                            skills[i].BeginCooldown(gameObject);
                            states[i] = SkillState.cooldown;
                            cooldownTime[i] = skills[i].cooldownTime;
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

        private void OnDisable()
        {
            for (int i = 0; i < skillsAction.Length;i++)
            {
                skillsAction[i].Disable();
            }
        }
    }
}
