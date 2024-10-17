using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    public class AI_Attack : MonoBehaviour
    {
        public float attackRange = 3f;         // Range within which the enemy can attack
        public float attackForce = 5f;          // Knockback force
        public float attackPower = 5f;
        public float attackCooldown = 0.5f;      // Cooldown between each attack
        public float comboResetTime = 1f;        // Cooldown after completing the combo
        public int maxCombo = 4;                 // Maximum combo count
        public bool canAttack = true;           // Can the AI attack

        public int currentCombo = 0;            // Tracks the current combo
        public float lastAttackTime = 0f;       // Time of the last attack

        private Transform player;
        private Rigidbody2D playerRigidbody;
        private AI_Movement aiMovement;
        private Animator anim;

        public AudioSource punchSound1;
        public AudioSource punchSound2;
        public AudioSource punchSound3;
        public AudioSource punchSound4;

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

                HealthBarTest playerHealth = player.GetComponent<HealthBarTest>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackPower);
                }

                // Debug.Log("Enemy performs attack : Attack" + (currentCombo));
                StartCoroutine(AttackCooldown());
                if (currentCombo > maxCombo)
                    {
                        currentCombo = 0;
                        StartCoroutine(ComboCooldown());
                    }

                AttackAnimation();
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
            switch (currentCombo)
            {
                case 1:
                    PlaySound(punchSound1);
                    anim.SetTrigger("attack1");
                    Debug.Log("Attack 1 triggered");
                    break;
                case 2:
                    PlaySound(punchSound2);
                    anim.SetTrigger("attack2");
                    Debug.Log("Attack 2 triggered");
                    break;
                case 3:
                    PlaySound(punchSound3);
                    anim.SetTrigger("attack3");
                    Debug.Log("Attack 3 triggered");
                    break;
                case 4:
                    PlaySound(punchSound4);
                    anim.SetTrigger("attack4");
                    Debug.Log("Attack 4 triggered");
                    Knockback();
                    break;
            }
        }

        private void PlaySound(AudioSource sound)
            {
                if (sound != null)
                {
                    sound.Play();
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
    }
}