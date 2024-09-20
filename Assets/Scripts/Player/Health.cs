using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int health = 100;

    // Method to take damage
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        // Check if health falls below zero
        if (health <= 0)
        {
            Die();
        }
    }

    // Handle what happens when the player dies
    private void Die()
    {
        Destroy(gameObject);
        Debug.Log($"{gameObject.name} has been defeated!");
    }

}
