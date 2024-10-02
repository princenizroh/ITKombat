using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Unity.Netcode;

public class HealthBar : NetworkBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public int maxHealth = 100;
    public float health;
    public float lerpSpeed = 5f;

    private void Start() 
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
    }

    void Update() 
    {
        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("Darah berkurang 10");
            TakeDamage(10);
        }

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Debug.Log("Anda mati!");
        }
    }
}
