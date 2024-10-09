using UnityEngine;
using UnityEngine.UI;

public class HealthBarTest : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public int maxHealth = 100;
    public float health = 100f;
    public float lerpSpeed = 5f;
    public string enemyTag = "Enemy"; 
    private int currentRound = 1;

    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed * Time.deltaTime);
        }
    }

    // Handle collision with enemy
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(enemyTag))
        {
            Debug.Log("Player collided with enemy: " + collision.gameObject.name);
            TakeDamage(10); 
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Taking {damage} damage. Current health: {health}");

        if (health <= 0)
        {
            Debug.Log("Mati!");
            if (currentRound == 1)
            {
                Debug.Log("Round 2");
                currentRound++;
                ResetHealth();
            }
            else
            {
                Debug.Log("Game Berakhir");
                ShowEndGameButton();
            }
        }
        UpdateHealthBar();
    }

    private void ResetHealth()
    {
        health = maxHealth;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthSlider.value = health;
    }

    private void ShowEndGameButton()
    {
        GameObject endGameButton = GameObject.Find("EndGameButton");
        if (endGameButton != null)
        {
            endGameButton.SetActive(true);
        }
    }
}
