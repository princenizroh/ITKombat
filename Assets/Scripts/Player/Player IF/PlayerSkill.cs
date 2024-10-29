using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;

namespace ITKombat
{
    public class PlayerSkill : MonoBehaviour
    {
        public AudioSource skillSound1;
        public AudioSource skillSound2;
        public AudioSource skillSound3;

        private Animator anim;
        private bool isSkill1Active = false;
        private bool isSkill2Active = false;
        private bool isSkill3Active = false;

        [SerializeField] private ParticleSystem Skill1_VFX_Right = null;
        [SerializeField] private ParticleSystem Skill2_VFX_Right = null;

        [SerializeField] private ParticleSystem Skill1_VFX_Left = null;
        [SerializeField] private ParticleSystem Skill2_VFX_Left = null;

        // Skill 3 Invisibility
        private SpriteRenderer character;
        private float activationTime;
        private bool invisible;
        private Color col;


        private void Start()
        {
            anim = GetComponent<Animator>();
            character = GetComponent<SpriteRenderer>();
            activationTime = 0;
            invisible = false;
            col = character.color;
        }

        public void Update()
        {
            activationTime += Time.deltaTime;
            if(invisible && activationTime >= 4)
            {
                invisible = false;
                col.a = 1;
                character.color = col;
            }
        }

        public void Skill1()
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;
            if (anim != null && !isSkill1Active)
            {
                if (character.IsFacingRight)
                {
                    PlayVFX(Skill1_VFX_Right);
                }
                else
                {
                    PlayVFX(Skill1_VFX_Left);
                }
                anim.SetTrigger("skill1");
                PlaySound(skillSound1);
                isSkill1Active = true;
                StartCoroutine(ResetToIdleAfterTime(1f)); 
            }
        }

        public void Skill2()
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;
            if (anim != null && !isSkill2Active)
            {
                if (character.IsFacingRight)
                {
                    PlayVFX(Skill2_VFX_Right);
                }
                else
                {
                    PlayVFX(Skill2_VFX_Left);
                }
                anim.SetTrigger("skill2");
                PlaySound(skillSound2);
                isSkill2Active = true;
                StartCoroutine(ResetToIdleAfterTime(1.2f)); 
            }
        }
        
        public void Skill3()
        {
            if (anim != null && !isSkill3Active)
            {
                anim.SetTrigger("skill3");
                invisible = true;
                activationTime = 0;
                col.a = .2f;
                character.color = col;
                PlaySound(skillSound3);
                isSkill3Active = true;
                StartCoroutine(ResetToIdleAfterTime(1.5f));
            }
        }

        private IEnumerator ResetToIdleAfterTime(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (anim != null)
            {
                anim.SetTrigger("idle");
            }

            isSkill1Active = false;
            isSkill2Active = false;
            isSkill3Active = false;
        }

        private void PlaySound(AudioSource sound)
        {
            if (sound != null && !sound.isPlaying)
            {
                sound.Play();
            }
        }
        private void PlayVFX(ParticleSystem vfx)
        {
            if (vfx != null && !vfx.isPlaying)
            {
                vfx.Play();
            }
        }
    }
}
