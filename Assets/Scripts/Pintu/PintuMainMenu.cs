using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PintuMainMenu : MonoBehaviour
{
    private bool isPlayerInRange = false; // Menyimpan status apakah player berada dalam collider

    // Fungsi ini dipanggil ketika sesuatu memasuki trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Memeriksa apakah yang masuk ke dalam collider adalah player
        if (other.CompareTag("player"))
        {
            isPlayerInRange = true; // Menandai bahwa player berada dalam range
        }
    }

    // Fungsi ini dipanggil ketika sesuatu keluar dari trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        // Memeriksa apakah yang keluar dari collider adalah player
        if (other.CompareTag("player"))
        {
            isPlayerInRange = false; // Menandai bahwa player tidak lagi dalam range
        }
    }

    // Fungsi ini dipanggil setiap frame
    private void Update()
    {
        // Memeriksa apakah player berada dalam range dan menekan tombol yang diinginkan (misalnya tombol "E")
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            LoadScene(); // Pindah ke scene lain
        }
    }

    // Fungsi untuk memuat scene main menu
    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
