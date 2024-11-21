using System;
using UnityEngine;

namespace ITKombat
{
    public class AnimasiPintu : MonoBehaviour
    {
        public Animator doorAnimator; // Referensi ke Animator pintu
        public string playerTag = "Player"; // Tag untuk mendeteksi pemain
        private bool isPlayerInside = false; // Untuk memastikan trigger hanya dipanggil sekali
        public SwordSwing swordSwing;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInside = true;
                Debug.Log("Ojek masuk");
                // Memicu animasi membuka pintu
                doorAnimator.SetTrigger("isOpen");
                Debug.Log("Animasi buka");
                if (swordSwing != null)
                {
                    swordSwing.TriggerSwing();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInside = false;
                Debug.Log("Ojek mkeluar");

                // Memicu animasi menutup pintu
                doorAnimator.SetTrigger("isClose");
                Debug.Log("Animasi tutup");
                StartCoroutine(ActivateTriggerWithDelay("isIdle", 0.5f)); // Sesuaikan delay dengan durasi animasi penutupan
                if (swordSwing != null)
                {
                    swordSwing.TriggerIdle();
                }
            }
        }

        private System.Collections.IEnumerator ActivateTriggerWithDelay(string triggerName, float delay)
        {
            // Menunggu selama 'delay' detik
            yield return new WaitForSeconds(delay);

            // Memicu animasi kembali ke state idle
            doorAnimator.SetTrigger(triggerName);
            Debug.Log("Animasi kembali ke idle");
        }
    }
}
