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
        public float attackRadius = 1f;
        public Transform attackPoint;
        public float attackCooldown = 1f;      // Cooldown between each attack
        public float comboResetTime = 1f;        // Cooldown after completing the combo
        public int maxCombo = 4;                 // Maximum combo count
        public bool canAttack = true;           // Can the AI attack

        public int currentCombo = 0;            // Tracks the current combo
        public float lastAttackTime = 0f;       // Time of the last attack

        public LayerMask playerlayer;
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
            // playerlayer = LayerMask.FindGameObjectWithTag("Player").transform;
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

                // Apply damage to player
                Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerlayer);
                foreach (Collider2D player in hitPlayer)
                {
                    Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
                    // PlayerDefense playerDefense = player.GetComponent<PlayerDefense>();
                    if (playerRB != null) //&& !playerDefense.isBlocking)
                    {   
                        GameObject playerStateObject = GameObject.FindGameObjectWithTag("PlayerState");
                        if (playerStateObject != null)
                        {
                            PlayerState playerState = playerStateObject.GetComponent<PlayerState>();
                            if (playerState != null)
                            {
                                if (currentCombo == 4) Knockback(player);
                                playerState.TakeDamage(attackPower);
                            }
                        }
                        // Debug.Log("Enemy performs attack : Attack" + (currentCombo));
                        StartCoroutine(AttackCooldown());
                        if (currentCombo > maxCombo)
                            {
                                currentCombo = 0;
                                StartCoroutine(ComboCooldown());
                            }
                        else
                        {
                            Debug.Log("PlayerState not found.");
                        }
                    }
                }
                AttackAnimation(hitPlayer);

            }
        }

        void Knockback(Collider2D playerCollider)
        {
            if (playerCollider != null)
            {
                Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // Calculate knockback direction from attackPoint to player
                    Vector2 knockbackDirection = (playerCollider.transform.position - attackPoint.position).normalized;

                    // Apply knockback force
                    playerRb.AddForce(knockbackDirection * attackForce, ForceMode2D.Impulse);
                    Debug.Log("Player hit by knockback");
                }
                else
                {
                    Debug.LogWarning("No Rigidbody2D found on the player.");
                }
            }
            else
            {
                Debug.LogWarning("No player detected within knockback radius.");
            }
        }


        private void AttackAnimation(Collider2D[] hitPlayer)
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
                    PlayAttackSound(1,hitPlayer.Length > 0);
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
                    PlayAttackSound(2,hitPlayer.Length > 0);
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
                    PlayAttackSound(3,hitPlayer.Length > 0);
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
                    PlayAttackSound(4,hitPlayer.Length > 0);
                    anim.SetTrigger("attack4");
                    Debug.Log("Attack 4 triggered");
                    break;
            }
        }

        private void PlayAttackSound(int comboNumber, bool hitPlayer)
        {
            if (hitPlayer)
            {
                PlayHitSound(comboNumber);
            }
            else
            {
                PlayMissSound(comboNumber);
            }
        }

        private void PlayHitSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: SoundManager.Instance.PlaySound3D("CharIF_Attack1", transform.position); break;
                case 2: SoundManager.Instance.PlaySound3D("CharIF_Attack2", transform.position); break;
                case 3: SoundManager.Instance.PlaySound3D("CharIF_Attack3", transform.position); break;
                case 4: SoundManager.Instance.PlaySound3D("CharIF_Attack4", transform.position); break;
            }
        }

        private void PlayMissSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: SoundManager.Instance.PlaySound3D("AttackMiss_noWeapon", transform.position); break;
                case 2: SoundManager.Instance.PlaySound3D("AttackMiss_noWeapon", transform.position); break;
                case 3: SoundManager.Instance.PlaySound3D("AttackMiss_noWeapon", transform.position); break;
                case 4: SoundManager.Instance.PlaySound3D("AttackMiss_noWeapon", transform.position); break;
            }
        }

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

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
