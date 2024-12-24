using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    public class MeleeBaseState : FelixState
    {
        // How long this state should be active for before moving on
        public float duration;
        // Cached animator component
        protected Animator animator;
        // bool to check whether or not the next attack in the sequence should be played or not
        protected bool shouldCombo;
        // The attack index in the sequence of attacks
        protected int attackIndex;

        // The cached hit collider component of this attack
        protected Collider2D hitCollider;
        // Cached already struck objects of said attack to avoid overlapping attacks on same target
        private List<Collider2D> collidersDamaged;
        // The Hit Effect to Spawn on the afflicted Enemy

        // Input buffer Timer
        private float AttackPressedTimer = 0;

        public override void OnEnter(FelixStateMachine _stateMachine)
        {
            base.OnEnter(_stateMachine);
            animator = GetComponent<Animator>();
            collidersDamaged = new List<Collider2D>();
            hitCollider = GetComponent<PlayerIFAttack>().hitbox;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            AttackPressedTimer -= Time.deltaTime;

            if (animator.GetFloat("Weapon.Active") > 0f)
            {
                Attack();
            }

            if (Input.GetMouseButtonDown(0))
            {
                AttackPressedTimer = 2;
            }

            if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0)
            {
                shouldCombo = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        protected void Attack()
        {
            Collider2D[] collidersToDamage = new Collider2D[10];
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            Debug.Log("ATTACK JALAN");
            bool isBlocked = false;

            if (hitCollider == null)
            {
                Debug.LogError("HitCollider is not assigned!");
                return;
            }
            
            int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

            for (int i = 0; i < colliderCount; i++)
            {
                Debug.Log("MENJALANKAN FOR");
                Collider2D targetCollider = collidersToDamage[i];

                if (!collidersDamaged.Contains(targetCollider))
                {
                    Debug.Log("Menjalankan IF 1");
                    GameObject enemy = targetCollider.gameObject;
                    TeamComponent hitTeamComponent = targetCollider.GetComponentInChildren<TeamComponent>();
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();

                    if (hitTeamComponent != null && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                    {
      
                        Debug.Log("Menjalankan IF 2");
                        if (enemyRb != null && (enemyDefense == null || !enemyDefense.isBlocking))
                        {
                            isBlocked = true;
                            Debug.Log("Menjalankan IF 3");
                            Debug.Log($"Enemy {targetCollider.name} has taken {attackIndex * 10} damage.");

                            GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");
                            Debug.Log("Jalan");
                            Debug.Log(enemyStateObject);

                            if (enemyStateObject != null)
                            {
                                Debug.Log("Menjalankan IF 4");
                                EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                                if (enemyState != null)
                                {
                                    enemyState.TakeDamage(attackIndex * 5);
                                    Debug.Log("Berhasil Kesini");

                                }
                            }
                            else
                            {
                                Debug.LogError("EnemyState object not found!");
                            }
                            
                            collidersDamaged.Add(targetCollider);
                            
                        }
                        PlayerIFAttack.Instance.PlayAttackSound(attackIndex, collidersToDamage.Length > 0, isBlocked);
                    }
            }
            
        }
        
        
    }

    }
}
