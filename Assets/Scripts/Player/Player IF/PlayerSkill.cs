using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;
using UnityEngine.Audio;

namespace ITKombat
{
    public class PlayerSkill : MonoBehaviour
    {
        public AudioSource skillSound1;
        public AudioSource skillSound2;
        public AudioSource skillSound3;

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
        private float skill1AttackRadius = 1f;
        private float skill1Damage = 30f;
        private float skill1Force = 5f;

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
        private float skill1CooldownTimer = 0f;
        private float skill2CooldownTimer = 0f;

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
            activationTime += Time.deltaTime;
            if(invisible && activationTime >= 4)
            {
                invisible = false;
                col.a = 1;
                character.color = col;
            }

            // Cooldown timers
            if (skill1CooldownTimer > 0)
            {
                skill1CooldownTimer -= Time.deltaTime;
            }

            if (skill2CooldownTimer > 0)
            {
                skill2CooldownTimer -= Time.deltaTime;
            }
        }

        public void Skill1()
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;
            if (skill1CooldownTimer <= 0 && anim != null && !isSkill1Active)
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
                skill1CooldownTimer = skill1Cooldown;

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, skill1AttackRadius, enemyLayer);
                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();
                    if (enemyRb != null && !enemyDefense.isBlocking)
                    {
                        enemyRb.AddForce(transform.right * skill1Force, ForceMode2D.Impulse);

                        GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");

                        if (enemyStateObject != null)
                        {
                            EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                            if (enemyState != null)
                            {
                                enemyState.TakeDamage(skill1Damage);
                            }
                        }
                        else
                        {
                            Debug.Log("EnemyState not found.");
                        }
                    }
                }

                StartCoroutine(ResetToIdleAfterTime(1f));
            }
            else
            {
                Debug.Log("Skill 1 masih cooldown.");
            }
        }

        public void Skill2()
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;
            if (skill2CooldownTimer <= 0 && anim != null && !isSkill2Active)
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
                shieldCounter = maxShieldHits;
                skill2CooldownTimer = skill2Cooldown;
                StartCoroutine(ResetToIdleAfterTime(1.2f)); 
            }
            else
            {
                Debug.Log("Skill 2 masih cooldown.");
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

        public void TakeDamage(float damage)
        {
            if (shieldCounter > 0)
            {
                shieldCounter--;  // Kurangi hitungan shield
                Debug.Log("Shield menahan damage, tersisa: " + shieldCounter);
                if (shieldCounter <= 0)
                {
                    isSkill2Active = false;  // Nonaktifkan shield jika hitungan habis
                }
            }
            else
            {
                // Jika tidak ada shield, terima damage seperti biasa
                PlayerState playerState = GetComponent<PlayerState>();
                if (playerState != null)
                {
                    playerState.TakeDamage(damage);
                }
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
