using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    public class AI_Attack : MonoBehaviour
    {
        public static AI_Attack Instance;
        public float attackRange = 2.5f;         // Range within which the enemy can attack
        public float attackForce = 5f;          // Knockback force
        public float attackPower = 5f;
        public float attackCooldown = 1f;      // Cooldown between each attack
        public float comboResetTime = 1f;        // Cooldown after completing the combo
        public int maxCombo = 4;                 // Maximum combo count
        public bool canAttack = true;           // Can the AI attack

        public int currentCombo = 0;            // Tracks the current combo
        public float lastAttackTime = 0f;       // Time of the last attack

        private Transform player;
        private Rigidbody2D playerRigidbody;
        private AI_Movement aiMovement;
        private Animator anim;

        // VFX Right
        [SerializeField] private ParticleSystem Attack1_Right = null;
        [SerializeField] private ParticleSystem Attack2_Right = null;
        [SerializeField] private ParticleSystem Attack3_Right = null;
        [SerializeField] private ParticleSystem Attack4_Right = null;

        // VFX Left
        [SerializeField] private ParticleSystem Attack1_Left = null;
        [SerializeField] private ParticleSystem Attack2_Left = null;
        [SerializeField] private ParticleSystem Attack3_Left = null;
        [SerializeField] private ParticleSystem Attack4_Left = null;

        // public AudioSource punchSound1;
        // public AudioSource punchSound2;
        // public AudioSource punchSound3;
        // public AudioSource punchSound4;

        private void Awake()
        {
            anim = GetComponent<Animator>();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

        }

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            aiMovement = GetComponent<AI_Movement>();  // Reference to AI_Movement
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            
        }

        public void Attack()
        {
            if (canAttack && Time.time - lastAttackTime > attackCooldown)
            {
                if (currentCombo == 0 || Time.time - lastAttackTime > attackCooldown*2)
                {
                    currentCombo = 1;
                }
                else
                {
                    currentCombo ++;
                }
                lastAttackTime = Time.time;

                GameObject playerState = GameObject.FindGameObjectWithTag("PlayerState");
                if (playerState != null)
                {
                    playerState.GetComponent<PlayerState>().TakeDamage(attackPower);
                }
                // Debug.Log("Enemy performs attack : Attack" + (currentCombo));
                StartCoroutine(AttackCooldown());
                if (currentCombo > maxCombo)
                    {
                        currentCombo = 0;
                        StartCoroutine(ComboCooldown());
                    }

                AttackAnimation();
                Debug.Log("Performed attack.");

            }
        }

        void Knockback()
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                Vector2 knockbackDirection = (player.position - transform.position).normalized;
                // playerRigidbody.linearVelocity = Vector2.zero;
                playerRigidbody.AddForce(knockbackDirection * (attackForce*500), ForceMode2D.Force);
                Debug.Log("Player hit by knockback");
            }
        }

        private void AttackAnimation()
        {
            CharacterController2D1 character = GetComponent<CharacterController2D1>();
            if (character == null) return;
            switch (currentCombo)
            {
                case 1:
                    if (character.IsFacingRight)
                    {
                        Attack1_Right.Play();
                    }
                    else
                    {
                        Attack1_Left.Play();
                    }
                    SoundManager.Instance.PlaySound3D("CharIF_Attack1", transform.position);
                    anim.SetTrigger("attack1");
                    Debug.Log("Attack 1 triggered");
                    break;
                case 2:
                    if (character.IsFacingRight)
                    {
                        Attack2_Right.Play();
                    }
                    else
                    {
                        Attack2_Left.Play();
                    }
                    SoundManager.Instance.PlaySound3D("CharIF_Attack2", transform.position);
                    anim.SetTrigger("attack2");
                    Debug.Log("Attack 2 triggered");
                    break;
                case 3:
                    if (character.IsFacingRight)
                    {
                        Attack3_Right.Play();
                    }
                    else
                    {
                        Attack3_Left.Play();
                    }
                    SoundManager.Instance.PlaySound3D("CharIF_Attack3", transform.position);
                    anim.SetTrigger("attack3");
                    Debug.Log("Attack 3 triggered");
                    break;
                case 4:
                    if (character.IsFacingRight)
                    {
                        Attack4_Right.Play();
                    }
                    else
                    {
                        Attack4_Left.Play();
                    }
                    SoundManager.Instance.PlaySound3D("CharIF_Attack4", transform.position);
                    anim.SetTrigger("attack4");
                    Debug.Log("Attack 4 triggered");
                    Knockback();
                    break;
            }
        }

        // private void PlaySound(AudioSource sound)
        //     {
        //         if (sound != null)
        //         {
        //             sound.Play();
        //         }
        //     }

        private IEnumerator AttackCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
        }

        private IEnumerator ComboCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(comboResetTime);
            Debug.Log("Combo Reset");
            // Reset the combo counter after cooldown
            // currentCombo = 0;
            canAttack = true;
        }
    }
}
