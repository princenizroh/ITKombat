using UnityEngine;
using TMPro;

namespace ITKombat
{
    public class MatchTimer : MonoBehaviour
    {
        public static MatchTimer Instance;
        public float timerSet = 0.5f;
        public float timerStart = 120;
        public float normalTimerSet = 0.5f;
        public float normalTimerStart;
        public bool statusMatch;

        private void Awake()
        {
            Instance = this;
        }

        void Start() {
            statusMatch = false;
        }

        void Update()
        {
            if (statusMatch == true) {

                if (timerStart >= timerSet)
                {
                    timerStart -= Time.deltaTime; 
                }

            } else {

                if (normalTimerStart >= normalTimerSet)
                {
                    normalTimerStart -= Time.deltaTime;
                }
                
            }
            
        }

        public void ChangeMatchStatus(bool status) {
            statusMatch = status;
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
