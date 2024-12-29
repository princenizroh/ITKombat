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

        private bool hasPlayedMissSound = false;


        public override void OnEnter(FelixStateMachine _stateMachine)
        {
            base.OnEnter(_stateMachine);
            animator = GetComponent<Animator>();
            collidersDamaged = new List<Collider2D>();
            hitCollider = GetComponent<PlayerIFAttack>()?.hitbox;
            
            if (hitCollider == null)
            {
                hitCollider = GetComponent<ServerCharacterAction>()?.hitbox;
            }
            else 
            {
                Debug.Log("HitCollider is not assigned!");
            }
            
            hasPlayedMissSound = false; // Reset flag
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
            hasPlayedMissSound = false; // Reset flag
        }

        private void DamagePlayerHandler()
        {

        }
        private void DamageEnemyHandler()
        {

        }
        protected void Attack()
        {
            Debug.Log("Attack method called");
            Collider2D[] collidersToDamage = new Collider2D[10];
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            Debug.Log("Initialized collidersToDamage and filter");
            bool isBlocked = false;

            if (hitCollider == null)
            {
                Debug.LogError("HitCollider is not assigned!");
                return;
            }
            
            int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);
            Debug.Log($"Number of colliders detected: {colliderCount}");




            for (int i = 0; i < colliderCount; i++)
            {
                Debug.Log($"Processing collider {i}");
                Collider2D targetCollider = collidersToDamage[i];

                if (!collidersDamaged.Contains(targetCollider))
                {
                    Debug.Log("Collider not previously damaged");
                    GameObject enemy = targetCollider.gameObject;
                    TeamComponent hitTeamComponent = targetCollider.GetComponentInChildren<TeamComponent>();
                    Debug.Log("Searching for TeamComponent" + hitTeamComponent);
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb == null)
                    {
                        Debug.Log("Enemy does not have Rigidbody2D");
                    }
                    Debug.Log("Enemy has Rigidbody2D");
                    AI_Defense enemyDefense = enemy.GetComponent<AI_Defense>();
                    PlayerDefense enemyPlayerDefense = enemy.GetComponent<PlayerDefense>();

                    
                    if (hitTeamComponent != null && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                    {
                        if (enemyDefense != null)
                        {
                            Debug.Log("Enemy has AI_Defense component");
                            Debug.Log("Target is an enemy");
                            if (enemyDefense != null && enemyDefense.isBlocking)
                            {
                                Debug.Log("Enemy is blocking");
                                isBlocked = true;
                            }

                            if (enemyRb != null && !enemyDefense.isBlocking)
                            {
                                Debug.Log("Enemy is not blocking and has Rigidbody2D");
                                Debug.Log($"Enemy {targetCollider.name} has taken {attackIndex * 10} damage.");

                                GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");


                                if (enemyStateObject != null)
                                {
                                    Debug.Log("EnemyState object found");
                                    EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                                    if (enemyState != null)
                                    {
                                        Debug.Log("EnemyState component found");
                                        enemyState.TakeDamage(attackIndex * 5);
                                        Debug.Log("Damage applied to enemy");
                                    }
                                }
                                else
                                {
                                    Debug.LogError("EnemyState object not found!");
                                }
                                
                                collidersDamaged.Add(targetCollider);
                                Debug.Log("Collider added to damaged list");
                            }
                        }
                        else if (enemyPlayerDefense != null && enemyPlayerDefense.CompareTag("Enemy"))
                        {
                            Debug.Log("Enemy has Player_Defense component");
                            Debug.Log("Target is an enemy");
                            if (enemyPlayerDefense != null && enemyPlayerDefense.isBlocking)
                            {
                                Debug.Log("Enemy is blocking");
                                isBlocked = true;
                            }

                            if (enemyRb != null)
                            {
                                Debug.Log("Enemy is not blocking and has Rigidbody2D");
                                Debug.Log($"Enemy {targetCollider.name} has taken {attackIndex * 10} damage.");

                                GameObject enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");


                                if (enemyStateObject != null)
                                {
                                    Debug.Log("EnemyState object found");
                                    EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                                    if (enemyState != null)
                                    {
                                        Debug.Log("EnemyState component found");
                                        // enemyState.TakeDamage(attackIndex * 5);
                                        enemyState.TakeDamageServerRpc(attackIndex * 5);
                                        Debug.Log("Damage applied to enemy");
                                    }
                                }
                                else
                                {
                                    Debug.LogError("EnemyState object not found!");
                                }
                                
                                collidersDamaged.Add(targetCollider);
                                Debug.Log("Collider added to damaged list");
                            }
                        }
                        else if (enemyPlayerDefense != null && enemyPlayerDefense.CompareTag("Player"))
                        {
                            Debug.Log("Enemy has Player_Defense component");
                            Debug.Log("Target is an enemy");
                            if (enemyPlayerDefense != null && enemyPlayerDefense.isBlocking)
                            {
                                Debug.Log("Enemy is blocking");
                                isBlocked = true;
                            }

                            if (enemyRb != null)
                            {
                                Debug.Log("Enemy is not blocking and has Rigidbody2D");
                                Debug.Log($"Enemy {targetCollider.name} has taken {attackIndex * 10} damage.");

                                GameObject playerStateObject = GameObject.FindGameObjectWithTag("PlayerState");


                                if (playerStateObject != null)
                                {
                                    Debug.Log("EnemyState object found");
                                    PlayerState playerState = playerStateObject.GetComponent<PlayerState>();
                                    if (playerState != null)
                                    {
                                        Debug.Log("EnemyState component found");
                                        // enemyState.TakeDamage(attackIndex * 5);
                                        playerState.TakeDamageServerRpc(attackIndex * 5, 1);
                                        Debug.Log("Damage applied to enemy");
                                    }
                                }
                                else
                                {
                                    Debug.LogError("EnemyState object not found!");
                                }
                                
                                collidersDamaged.Add(targetCollider);
                                Debug.Log("Collider added to damaged list");
                            }
                        }
                        
                        // PlayerIFAttack.Instance.PlayAttackSound(attackIndex, collidersToDamage.Length > 0, isBlocked);
                        
                    }
                    
                }
            }

            // if (colliderCount == 0 && !hasPlayedMissSound)
            // {
            //     PlayerIFAttack.Instance.PlayMissSound(attackIndex); // Memainkan suara pukulan meleset
            //     hasPlayedMissSound = true; // Set flag agar suara tidak diputar ulang
            // }
            
        }
    }
}