using System.Collections;
using TMPro; // Namespace untuk TextMeshPro
using UnityEngine;

public class CounterTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // Ganti tipe data ke TextMeshProUGUI
    private int timeElapsed; // Waktu yang telah berlalu dalam detik

    void Start()
    {
        timeElapsed = 0; // Mulai dari 0 detik
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (true) // Timer akan berjalan terus menerus
        {
            int minutes = timeElapsed / 60;
            int seconds = timeElapsed % 60;
            countdownText.text = $"{minutes}:{seconds:D2}";
            yield return new WaitForSeconds(1); 
            timeElapsed++; 
        }
    }
}
