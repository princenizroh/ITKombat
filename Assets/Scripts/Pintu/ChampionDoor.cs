using UnityEngine;
using System.Collections;

namespace ITKombat
{
    public class ChampionDoor : MonoBehaviour
    {
        public Animator doorAnimator; // Referensi ke Animator pintu
        public string playerTag = "Player"; // Tag untuk mendeteksi pemain
        private bool isPlayerInside = false; // Untuk memastikan trigger hanya dipanggil sekali
        public ChampionLogo logoSwing;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(playerTag) && !isPlayerInside)
            {
                isPlayerInside = true;
                Debug.Log("Player masuk ke area pintu.");

                // Memicu animasi membuka pintu
                doorAnimator.SetTrigger("isOpen");
                Debug.Log("Trigger 'isOpen' dijalankan.");
                if (logoSwing != null)
                {
                    logoSwing.TriggerSwing();
                }

                // Menunggu sejenak sebelum menjalankan trigger 'isOpenIdle'
                StartCoroutine(ActivateTriggerWithDelay("isOpenIdle", GetAnimationClipLength("isOpen")));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(playerTag) && isPlayerInside)
            {
                isPlayerInside = false;
                Debug.Log("Player keluar dari area pintu.");

                // Memicu animasi menutup pintu
                doorAnimator.SetTrigger("isClose");
                Debug.Log("Trigger 'isClose' dijalankan.");
                if (logoSwing != null)
                {
                    logoSwing.TriggerIdle();
                }

                // Menunggu sejenak sebelum menjalankan trigger 'isIdle'
                StartCoroutine(ActivateTriggerWithDelay("isIdle", GetAnimationClipLength("isClose")));
            }
        }

        private IEnumerator ActivateTriggerWithDelay(string triggerName, float delay)
        {
            // Menunggu selama 'delay' detik
            yield return new WaitForSeconds(delay);

            // Memicu animasi berikutnya
            doorAnimator.SetTrigger(triggerName);
            Debug.Log($"Trigger '{triggerName}' dijalankan setelah delay {delay} detik.");
        }

        private float GetAnimationClipLength(string triggerName)
        {
            // Mendapatkan durasi animasi berdasarkan nama trigger
            if (doorAnimator.runtimeAnimatorController != null)
            {
                foreach (var clip in doorAnimator.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == triggerName)
                    {
                        return clip.length;
                    }
                }
            }
            Debug.LogWarning($"Animation clip dengan nama '{triggerName}' tidak ditemukan.");
            return 0.5f; // Default delay jika animasi tidak ditemukan
        }
    }
}
