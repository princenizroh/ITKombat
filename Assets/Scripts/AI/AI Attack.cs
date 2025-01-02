using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    public class AI_Attack : MonoBehaviour
    {
        public static AI_Attack Instance;
        public float attackRange = 0.35f;         // Range within which the enemy can attack
        public float attackPower = 3f;
        public float attackRadius = 0.2f;
        public Transform attackPoint;
        public float attackCooldown = 0.7f;      // Cooldown between each attack
        public float comboResetTime = 1f;        // Cooldown after completing the combo
        public int maxCombo = 4;                 // Maximum combo count
        public bool canAttack = true;           // Can the AI attack
        public bool newCanAttack = true;

        public int currentCombo = 0;            // Tracks the current combo
        public float lastAttackTime = 0f;       // Time of the last attack

        public LayerMask playerlayer;
        private Animator anim;
        private bool isBlocked = false;

        // // VFX Right
        // [SerializeField] private ParticleSystem Attack1_Right = null;
        // [SerializeField] private ParticleSystem Attack2_Right = null;
        // [SerializeField] private ParticleSystem Attack3_Right = null;
        // [SerializeField] private ParticleSystem Attack4_Right = null;

        // // VFX Left
        // [SerializeField] private ParticleSystem Attack1_Left = null;
        // [SerializeField] private ParticleSystem Attack2_Left = null;
        // [SerializeField] private ParticleSystem Attack3_Left = null;
        // [SerializeField] private ParticleSystem Attack4_Left = null;

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
        }

        void Update()
        {
            
        }

        public void Attack()
        {
            if (newCanAttack && canAttack && Time.time - lastAttackTime > attackCooldown)
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
                bool isBlocked = false;

                // Apply damage to player
                Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerlayer);
                foreach (Collider2D player in hitPlayer)
                {
                    Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
                    PlayerMovement_2 playerMovement = player.GetComponent<PlayerMovement_2>();
                    
                        if(playerMovement != null && playerMovement.isBlocking){
                            isBlocked = true;
                        }

                        if (playerRB != null && !playerMovement.isBlocking)
                        {
                            GameObject playerStateObject = GameObject.FindGameObjectWithTag("PlayerState");
                            if (playerStateObject != null)
                            {
                                PlayerState playerState = playerStateObject.GetComponent<PlayerState>();
                                if (playerState != null)
                                {
                                    // if(!playerMovement.isBlocking){
                                        ApplyKnockback(player,currentCombo);
                                        playerState.TakeDamage(attackPower,currentCombo);
                                    // }
                                }
                                else
                                {
                                    Debug.Log("PlayerState not found.");
                                }
                            }
                        }
                    // } 
                    // else 
                    // {
                    //     Debug.Log("PlayerMovement not found.");
                    // }
                    AttackAnimation(hitPlayer, isBlocked);
                }
                
                // AttackAnimation(hitPlayer);

                // Debug.Log("Enemy performs attack : Attack" + currentCombo);
                StartCoroutine(AttackCooldown());
                if (currentCombo >= maxCombo)
                    {
                        currentCombo = 0;
                        StartCoroutine(ComboCooldown());
                    }
            }
            else
            {
                 anim.SetTrigger("Idle");
            }
        }

        public bool GetCanAttack (bool CanAttack)
        {
            newCanAttack = CanAttack;
            Debug.Log("Berhasil Get AI Can Attack = " + CanAttack);
            return newCanAttack;
        }

        void ApplyKnockback(Collider2D playerCollider, float currentCombo)
        {
            if (playerCollider != null)
            {
                Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    if (currentCombo == 4) // knockback hanya untuk hit ke 4
                        {
                            float attackForce = playerCollider.bounds.size.magnitude; // penyesuaian attack force sesuai size karakter
                            Vector2 direction = (playerCollider.transform.position - attackPoint.position).normalized; // penyesuaian arah knockback

                            playerRb.AddForce(direction * attackForce, ForceMode2D.Force);
                        }
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
         private void AttackAnimation(Collider2D[] hitPlayer, bool isBlocked)
        {
            switch (currentCombo)
            {
                case 1:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack1_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack1_Left.Play();
                    // }
                    anim.SetTrigger("Attack1");
                    soundPlayerIF.Instance.PlayAttackSound(1,hitPlayer.Length > 0, isBlocked);
                    // Debug.Log("Attack 1 triggered");
                    break;
                case 2:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack2_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack2_Left.Play();
                    // }
                    anim.SetTrigger("Attack2");
                    soundPlayerIF.Instance.PlayAttackSound(2,hitPlayer.Length > 0, isBlocked);
                    // Debug.Log("Attack 2 triggered");
                    break;
                case 3:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack3_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack3_Left.Play();
                    // }
                    anim.SetTrigger("Attack3");
                    soundPlayerIF.Instance.PlayAttackSound(3,hitPlayer.Length > 0, isBlocked);
                    // Debug.Log("Attack 3 triggered");
                    break;
                case 4:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack4_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack4_Left.Play();
                    // }
                    anim.SetTrigger("Attack4");
                    soundPlayerIF.Instance.PlayAttackSound(4,hitPlayer.Length > 0, isBlocked);
                    // Debug.Log("Attack 4 triggered");
                    break;
            }
        }
        
        // private void AttackAnimation(Collider2D[] hitPlayer, bool isBlocked)
        // {
        //     CharacterController2D1 character = GetComponent<CharacterController2D1>();
        //     if (character == null) return;
        //     switch (currentCombo)
        //     {
        //         case 1:
        //             if (character.IsFacingRight)
        //             {
        //                 Attack1_Right.Play();
        //             }
        //             else
        //             {
        //                 Attack1_Left.Play();
        //             }
        //             PlayAttackSound(1,hitPlayer.Length > 0, isBlocked);
        //             anim.SetTrigger("Attack1");
        //             // Debug.Log("Attack 1 triggered");
        //             break;
        //         case 2:
        //             if (character.IsFacingRight)
        //             {
        //                 Attack2_Right.Play();
        //             }
        //             else
        //             {
        //                 Attack2_Left.Play();
        //             }
        //             PlayAttackSound(2,hitPlayer.Length > 0, isBlocked);
        //             anim.SetTrigger("Attack2");
        //             // Debug.Log("Attack 2 triggered");
        //             break;
        //         case 3:
        //             if (character.IsFacingRight)
        //             {
        //                 Attack3_Right.Play();
        //             }
        //             else
        //             {
        //                 Attack3_Left.Play();
        //             }
        //             PlayAttackSound(3,hitPlayer.Length > 0, isBlocked);
        //             anim.SetTrigger("Attack3");
        //             // Debug.Log("Attack 3 triggered");
        //             break;
        //         case 4:
        //             if (character.IsFacingRight)
        //             {
        //                 Attack4_Right.Play();
        //             }
        //             else
        //             {
        //                 Attack4_Left.Play();
        //             }
        //             PlayAttackSound(4,hitPlayer.Length > 0, isBlocked);
        //             anim.SetTrigger("Attack4");
        //             // Debug.Log("Attack 4 triggered");
        //             break;
        //     }
        // }

        // private void PlayAttackSound(int comboNumber, bool hitPlayer, bool isBlocked)
        // {
        //     if (isBlocked == true)
        //     {
        //         PlayBlockedSound(comboNumber);
        //         return;
        //     }
            
        //     if (hitPlayer)
        //     {
        //         PlayHitSound(comboNumber);
        //         return;
        //     }
        //     PlayMissSound(comboNumber);
            
        // }

        // private void PlayHitSound(int comboNumber)
        // {
        //     switch (comboNumber)
        //     {
        //         case 1: NewSoundManager.Instance.PlaySound("IF_Attack1", transform.position); break;
        //         case 2: NewSoundManager.Instance.PlaySound("IF_Attack2", transform.position); break;
        //         case 3: NewSoundManager.Instance.PlaySound("IF_Attack3", transform.position); break;
        //         case 4: NewSoundManager.Instance.PlaySound("IF_Attack4", transform.position); break;
        //     }
        // }

        // private void PlayMissSound(int comboNumber)
        // {
        //     switch (comboNumber)
        //     {
        //         case 1: NewSoundManager.Instance.PlaySound("Attack_Miss1", transform.position); break;
        //         case 2: NewSoundManager.Instance.PlaySound("Attack_Miss2", transform.position); break;
        //         case 3: NewSoundManager.Instance.PlaySound("Kick_Miss", transform.position); break;
        //         case 4: NewSoundManager.Instance.PlaySound("IF_Attack4", transform.position); break;
        //     }
        // }

        // private void PlayBlockedSound(int comboNumber)
        // {
        //     switch (comboNumber)
        //     {
        //         case 1: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
        //         case 2: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
        //         case 3: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
        //         case 4: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
        //     }
        // }

        public IEnumerator AttackCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
        }

        private IEnumerator ComboCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(comboResetTime);
            canAttack = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
