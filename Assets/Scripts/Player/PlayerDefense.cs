using ITKombat;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITKombat
{
    public class PlayerDefense : MonoBehaviour
    {
        [Header("Defense Settings")]
        public float blockCooldown = 1.0f;
        private bool isBlocking = false;


        [Header("Parry Settings")]
        public float parryWindow = 0.2f;
        public int parryDamage = 10;
        private float lastBlockTime = -1f;
        private bool canParry = false;
        private bool isParrying = false;

        [Header("Colliders")]
        public Transform defensePoint;
        public LayerMask enemyLayer;
        public float defenseRadius = 1f;

        [Header("Input Actions")]
        private InputActionAsset inputActionAsset;
        private InputAction defenseAction;

        Animator anim;
        PlayerControls controls;
        private GameObject parentPlayer;

        private void Awake()
        {
            // Finding Input Action for the Attack
            var playerActionMap = inputActionAsset.FindActionMap("Player");
            playerActionMap.Enable();
            defenseAction = playerActionMap.FindAction("Defense");

            // Input Action Scripts
            controls = new PlayerControls();
            controls.Enable();
            defenseAction = controls.Player.Defense;

            defenseAction.performed += ctx => StartBlocking();
            defenseAction.canceled += ctx => EndBlocking();
        }

        private void Start()
        {
            // Get parent GameObject
            parentPlayer = transform.root.gameObject;
            anim = GetComponent<Animator>();
        }

        public void StartBlocking()
        {
            isBlocking = true;
            lastBlockTime = Time.time;
            Debug.Log(gameObject.name + " started blocking.");
        }

        public void EndBlocking()
        {
            isBlocking = false;
            isParrying = false; // Reset parry ketika block selesai
            Debug.Log(gameObject.name + " stopped blocking.");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if the collider is an attack
            if (collision.CompareTag("Attack"))
            {
                // Ensure the attack is from a different player
                GameObject otherParent = collision.transform.root.gameObject;

                if (otherParent != parentPlayer && otherParent.CompareTag("Player"))
                {
                    // Check if the player is blocking
                    if (isBlocking)
                    {
                        // Block the attack succesfully
                        Debug.Log(gameObject.name + " blocked the attack.");
                        BlockSuccess(collision);

                        // Check jika parry bisa dilakukan saat dalam jangka parry window
                        if (Time.time - lastBlockTime <= parryWindow)
                        {
                            PerformParry(collision);
                        }
                    }
                    else
                    {
                        // Player is not blocking, take damage
                        Debug.Log(gameObject.name + " was hit by " + otherParent.name);
                        TakeDamage(collision.gameObject.GetComponent<PlayerAttack>().damageAmount);
                    }
                }
            }
        }

        // Logic for what happens when the block is successful
        private void BlockSuccess(Collider2D attack)
        {
            // Masih bisa naruh block anim/sound
            Debug.Log("Attack blocked!");

            // Destroy the attack or disable
            Destroy(attack.gameObject);
        }

        private void PerformParry(Collider2D attack)
        {
            isParrying = true;
            Debug.Log("Perfect parry! PUKULIN!");

            //Memberikan damage saat parry
            //Note tambahan: Jadi di bagian bawah ini, masih dalam review
            //Soalnya, variable attack di PlayerAttack.cs belum kedefine
            PlayerAttack enemyAttack = attack.gameObject.GetComponent<PlayerAttack>();
            if (enemyAttack != null)
            {
                HealthBar enemyHealth = enemyAttack.GetComponent<HealthBar>();
                if (enemyHealth != null)
                {
                    // enemyHealth.TakeDamage(parryDamage);
                    Debug.Log("Enemy took " + parryDamage + " damage from parry!");
                }
            }
        }

        // Logic for taking damage
        public void TakeDamage(int damageAmount)
        {
            Health health = GetComponent<Health>();
            if (health != null)
            {
                /*            health.TakeDamage(damageAmount);*/
                Debug.Log($"{gameObject.name} took {damageAmount} damage!");
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (defensePoint == null)
                return;

            Gizmos.DrawWireSphere(defensePoint.position, defenseRadius);
        }
    }

}