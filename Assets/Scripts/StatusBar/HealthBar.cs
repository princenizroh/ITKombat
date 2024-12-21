using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class HealthBar : MonoBehaviour
    {
        public static HealthBar Instance;
        public Slider healthSlider;
        public Slider easeSlider;
        public float lerpSpeed = 5f;

        private void Update()
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, healthSlider.value, lerpSpeed * Time.deltaTime);
        }


        void Awake() {

            Instance = this;

        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            if (maxHealth > 0) // Cek maxHealth untuk menghindari pembagian dengan nol
            {
                healthSlider.value = currentHealth / maxHealth;
            }
        }


        public void SetMaxHealth(float maxHealth)
        {
            healthSlider.maxValue = 1;
            easeSlider.maxValue = 1;
            healthSlider.value = 1;
            easeSlider.value = 1;
        }
    }
}
