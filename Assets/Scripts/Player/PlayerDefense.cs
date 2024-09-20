using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDefense : MonoBehaviour
{
    [Header("Defense Settings")]
    public float parryWindow = 0.2f;
    public float blockCooldown = 1.0f;

    private bool isBlocking = false;
    private bool isParrying = false;
    private bool canParry = false;

    [Header("Parry Settings")]
    public int parryDamage = 10;

    [Header("Colliders")]
    public Transform defensePoint;
    public LayerMask enemyLayer;
    public float defenseRadius = 1f;

    [Header("Input Actions")]
    private InputActionAsset inputActionAsset;
    private InputAction defenseAction;

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
    }

    public void StartBlocking()
    {
        isBlocking = true;
        Debug.Log(gameObject.name + " started blocking.");
    }

    public void EndBlocking()
    {
        isBlocking = false;
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
    
    // Logic for taking damage
    public void TakeDamage(int damageAmount)
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (defensePoint == null)
            return;

        Gizmos.DrawWireSphere(defensePoint.position, defenseRadius);
    }
}
