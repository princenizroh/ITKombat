using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Variabel untuk menyimpan nilai health pemain
    public int maxHealth = 100;
    private int currentHealth;

    // Start adalah fungsi yang dipanggil saat awal game
    void Start()
    {
        // Set health awal ke nilai maksimal
        currentHealth = maxHealth;
    }

    // Update adalah fungsi yang dipanggil setiap frame
    void Update()
    {
        // Pengecekan apakah tombol H ditekan untuk mengurangi health
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Kurangi health pemain
            TakeDamage(10);
        }

        // Pengecekan apakah tombol R ditekan untuk mereset health
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetHealth();
        }
    }

    // Fungsi untuk mengurangi health
    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0){
            currentHealth = 0;
        }


        Debug.Log("Debug Key(H) ditekan, Health sekarang: " + currentHealth);

        // Jika health pemain habis
        if (currentHealth == 0)
        {
            Die();
        }
    }

    // Fungsi untuk mengatur ulang health ke nilai maksimal
    void ResetHealth()
    {
        currentHealth = maxHealth;
        Debug.Log("Health di-reset: " + currentHealth);
    }

    // Fungsi untuk menangani kematian pemain
    void Die()
    {
        Debug.Log("Pemain mati!");
        // Lakukan logika kematian di sini, seperti menampilkan game over atau reset level
    }
}
