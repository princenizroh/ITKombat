using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;
using UnityEngine.Audio;
using Unity.Burst.Intrinsics;

namespace ITKombat
{
    public class PlayerSkill : MonoBehaviour
    {
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
                // Trigger VFX based on direction
                if (character.IsFacingRight)
                {
                    PlayVFX(Skill1_VFX_Right);
                }
                else
                {
                    PlayVFX(Skill1_VFX_Left);
                }

                anim.SetTrigger("skill1");
                isSkill1Active = true;
                PlaySound(skillSound1);
                skill1CooldownTimer = skill1Cooldown;

                // Detect enemies in range
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, skill1AttackRadius, enemyLayer);
                foreach (Collider2D enemy in hitEnemies)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();

                    if (enemyRb != null && (enemyDefense == null))
                    {
                        // Apply force to enemy
                        Vector2 forceDirection = character.IsFacingRight ? Vector2.right : Vector2.left;
                        enemyRb.AddForce(forceDirection * skill1Force, ForceMode2D.Impulse);

                        // Apply damage to the enemy
                        GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");
                        if (enemyStateObject != null)
                        {
                            EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();

                            if (enemyState != null)
                            {
                                enemyState.TakeDamageFromSkill(skill1Damage);
                            }
                        }
                        else
                        {
                            Debug.Log("EnemyState component not found on enemy.");
                        }
                    }
                }

                StartCoroutine(ResetToIdleAfterTime(1f));
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
                shieldCounter = maxShieldHits;
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
            if (isSkill2Active && shieldCounter > 0)
            {
                shieldCounter--; // Reduce shield hits
                Debug.Log("Shield blocked damage. Remaining hits: " + shieldCounter);

                if (shieldCounter <= 0)
                {
                    isSkill2Active = false; // Deactivate shield when hits are exhausted
                    Debug.Log("Shield is now inactive.");
                }
                return; // Do not apply damage
            }

            // Apply damage if no shield is active
            PlayerState playerState = GetComponent<PlayerState>();
            if (playerState != null)
            {
                playerState.TakeDamageFromSkill(damage);
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
