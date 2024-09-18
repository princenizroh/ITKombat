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
            int minutes = countdown / 60;
            int seconds = countdown % 60;
            countdownText.text = $"{minutes}:{seconds:D2}";
            yield return new WaitForSeconds(1);
            countdown--;
        }

        countdownText.text = "Waktu Habis!";
    }
}
