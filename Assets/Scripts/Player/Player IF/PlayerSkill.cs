using UnityEngine;
using System.Collections;

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

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void Skill1()
        {
            if (anim != null && !isSkill1Active)
            {
                anim.SetTrigger("skill1");
                PlaySound(skillSound1);
                isSkill1Active = true;
                StartCoroutine(ResetToIdleAfterTime(1f)); 
            }
        }

        public void Skill2()
        {
            if (anim != null && !isSkill2Active)
            {
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
    }
}
