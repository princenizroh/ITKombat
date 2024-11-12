using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace ITKombat
{
    public class IF_AISkill : MonoBehaviour
    {
        public float skill1Chance = 0.3f;
        public float skill3Chance = 0.3f;
        public Transform player;
        public SkillsHolder skillsHolder;
        public AI_Movement aiMovement;
        public AI_Attack aiAttack;
        public GameObject enemyStateObject;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            skillsHolder = GetComponent<SkillsHolder>();
            aiMovement = GetComponent<AI_Movement>();
            aiAttack = GetComponent<AI_Attack>();
            enemyStateObject = GameObject.FindGameObjectWithTag("EnemyState");
        }

        void Update()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            // Debug.Log(distanceToPlayer);
            float chance = Random.value;

            if (distanceToPlayer <= aiAttack.attackRange && chance < skill1Chance)
            {
                StartCoroutine(ActivatingSkill1());
            }

            if (enemyStateObject != null)
            {
                EnemyState enemyState = enemyStateObject.GetComponent<EnemyState>();
                if (enemyState != null)
                {
                    if (enemyState.currentHealth <= enemyState.maxHealth * 0.4f)
                    {
                        skillsHolder.ActivateSkill2();
                    }

                    if (distanceToPlayer >= aiMovement.maxDistance && chance < skill3Chance)
                    {
                        skillsHolder.ActivateSkill3();
                    }
                }
            }
        }   

        IEnumerator ActivatingSkill1()
        {
            aiAttack.canAttack = false;
            skillsHolder.ActivateSkill1();
            yield return new WaitForSeconds(1);
            aiAttack.canAttack = true;
        }
    }
}
