using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public int CountDown;

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int countdown = CountDown; 

        while (countdown >= 0) 
        {
            countdownText.text = $"{countdown:0}"; 
            yield return new WaitForSeconds(1); 
            countdown--; 
        }

        countdownText.text = "Waktu Habis!"; 
    }
}
