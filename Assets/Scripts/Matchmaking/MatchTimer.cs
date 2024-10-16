using UnityEngine;
using TMPro;

namespace ITKombat
{
    public class MatchTimer : MonoBehaviour
    {
        public float timerSet = 0.5f;
        public float timerStart = 120;
        public float normalTimerSet = 0.5f;
        public float normalTimerStart;

        void Update()
        {
            if (timerStart >= timerSet)
            {
                timerStart -= Time.deltaTime; 
            }

            if (normalTimerStart >= normalTimerSet)
            {
                normalTimerStart -= Time.deltaTime;
            }
        }

        public int GetStageTimeInMinute()
        {
            int minutes = Mathf.FloorToInt(timerStart / 60);
            return minutes;
        }

        public int GetStageTimeInSecond()
        {
            return Mathf.FloorToInt(timerStart);
        }

        public int GetNormalTimeInSecond()
        {
            return Mathf.FloorToInt(normalTimerStart);
        }
    }
}
