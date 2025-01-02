using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;
using UnityEngine.Audio;
using Unity.Burst.Intrinsics;

namespace ITKombat
{
    public class PlayerMesin_Skill : MonoBehaviour
    {
        [SerializeField] private MesinSkill1 skill1Asset;
        [SerializeField] public MesinSkill2 skill2Asset;
        [SerializeField] private MesinSkill3 skill3Asset;

        public AudioSource skillSound1;
        public AudioSource skillSound2;
        public AudioSource skillSound3;
        public AudioSource skillSound3Part2;

        private Animator anim;
        private bool isSkill1Active = false;
        public bool isSkill2Active = false;
        private bool isSkill3Active = false;

        [SerializeField] private ParticleSystem Skill1_VFX_Right = null;
        [SerializeField] private ParticleSystem Skill2_VFX_Right = null;

        [SerializeField] private ParticleSystem Skill1_VFX_Left = null;
        [SerializeField] private ParticleSystem Skill2_VFX_Left = null;

        // Skill 1 Damage
        [SerializeField] private Transform attackPoint;
        [SerializeField] public Collider2D hitbox;

        // Skill 2 Shield
        private int shieldCounter = 0;
        private const int maxShieldHits = 2;

        // Skill 3 Invisibility
        private SpriteRenderer character;
        private float activationTime;
        private bool invisible;
        private Color col;

        // Cooldown Settings
        private float skill1Cooldown = 10f;
        private float skill2Cooldown = 15f;
        private float skill3Cooldown = 20f;
        private float skill1CooldownTimer = 0f;
        private float skill2CooldownTimer = 0f;
        private float skill3CooldownTimer = 0f;

        public LayerMask enemyLayer;


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
            // Cooldown timers
            if (skill1CooldownTimer > 0)
            {
                skill1CooldownTimer -= Time.deltaTime;
            }

            if (skill2CooldownTimer > 0)
            {
                skill2CooldownTimer -= Time.deltaTime;
            }

            if (skill3CooldownTimer > 0)
            {
                skill3CooldownTimer -= Time.deltaTime;
            }
        }

        public void Skill1()
        {
            if (skill1CooldownTimer <= 0 && anim != null && !isSkill1Active)
            {
                // Trigger VFX based on direction
                CharacterController2D1 character = GetComponent<CharacterController2D1>();
                if (character != null)
                {
                    if (character.IsFacingRight)
                    {
                        PlayVFX(Skill1_VFX_Right);
                    }
                    else
                    {
                        PlayVFX(Skill1_VFX_Left);
                    }
                }

                anim.SetTrigger("skill1");
                isSkill1Active = true;
                PlaySound(skillSound1);

                // Activate the skill from the ScriptableObject
                skill1Asset.Activate(gameObject);
            }
            else
            {
                Debug.Log("Skill 1 is on cooldown.");
            }
        }

        public void Skill2()
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;

            if (skill2CooldownTimer <= 0 && anim != null && !isSkill2Active)
            {
                // Trigger VFX based on direction
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
                // Activate the skill from the ScriptableObject
                skill2Asset.Activate(gameObject);

                skill2CooldownTimer = skill2Cooldown;

                Debug.Log("Shield activated. Max hits: " + maxShieldHits);

                StartCoroutine(ResetToIdleAfterTime(1.2f));
            }
            else
            {
                Debug.Log("Skill 2 is on cooldown.");
            }
        }

        public void Skill3()
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;

            if (skill3CooldownTimer <= 0 && anim != null && !isSkill3Active)
            {
                // Trigger VFX based on direction
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
                // Activate the skill from the ScriptableObject
                skill2Asset.Activate(gameObject);

                skill2CooldownTimer = skill2Cooldown;

                Debug.Log("Shield activated. Max hits: " + maxShieldHits);

                StartCoroutine(ResetToIdleAfterTime(1.2f));
            }
            else
            {
                Debug.Log("Skill 2 is on cooldown.");
            }
        }

        private IEnumerator ResetToIdleAfterTime(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (anim != null)
            {
                anim.SetTrigger("Idle");
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
