using UnityEngine;
using TMPro;

namespace ITKombat
{
    public class MatchTimer : MonoBehaviour
    {
        public static MatchTimer Instance { get; private set; }
        [SerializeField] private float timerSet = 0.5f;
        [SerializeField] private float timerStart = 120;
        [SerializeField] private float normalTimerSet = 0.1f;
        [SerializeField] private float normalTimerStart = 5f;
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

        public int GetResetTimerStart()
        {
            timerStart = 120;
            return Mathf.FloorToInt(timerStart);
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
