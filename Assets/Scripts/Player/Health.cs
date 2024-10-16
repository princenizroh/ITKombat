using UnityEngine;

namespace ITKombat
{
    public class Health : MonoBehaviour
    {
        public int health = 100;

        // Method to take damage
        public void TakeDamage(int damageAmount)
        {
            health -= damageAmount;

            // Debug log to indicate damage taken
            Debug.Log($"{gameObject.name} took {damageAmount} damage! Remaining health: {health}");

            // Check if health falls below zero
            if (health <= 0)
            {
                Die();
            }
        }

        // Handle what happens when the player dies
        private void Die()
        {
            Debug.Log($"{gameObject.name} has been defeated!");
            Destroy(gameObject); // Replace with any custom death behavior, respawn logic, etc.
        }
    }
}
