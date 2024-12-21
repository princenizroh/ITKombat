using System.Collections;
using TMPro; 
using UnityEngine;

public class CounterTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    private int timeElapsed; 
    void Start()
    {
        timeElapsed = 0; 
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (true) 
        {
            int minutes = timeElapsed / 60;
            int seconds = timeElapsed % 60;
            countdownText.text = $"{minutes}:{seconds:D2}";
            yield return new WaitForSeconds(1); 
            timeElapsed++; 
        }
    }
}
