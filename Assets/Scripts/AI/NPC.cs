using System.Collections;
using UnityEngine;

namespace ITKombat
{
    public class NPCPatrol : MonoBehaviour
    {
        public float minPatrolDistance = 3f;     // Jarak minimum NPC berjalan
        public float maxPatrolDistance = 6f;     // Jarak maksimum NPC berjalan
        public float speed = 1f;                 // Kecepatan gerak NPC
        public float minWaitTime = 1f;           // Waktu tunggu minimum
        public float maxWaitTime = 3f;           // Waktu tunggu maksimum
        public float continueChance = 0.5f;      // Kemungkinan NPC melanjutkan arah yang sama

        private Vector3 startPosition;           // Posisi awal NPC
        private Vector3 targetPosition;          // Posisi target untuk pergerakan saat ini
        private bool movingRight = true;         // Arah awal (bergerak ke kanan)
        private SpriteRenderer spriteRenderer;   // Komponen untuk mengatur flip sprite

        private void Start()
        {
            startPosition = transform.position;  // Menyimpan posisi awal
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetRandomTargetPosition();
            StartCoroutine(Patrol());
        }

        private void SetRandomTargetPosition()
        {
            // Tentukan target posisi dengan jarak acak antara minPatrolDistance dan maxPatrolDistance
            float patrolDistance = Random.Range(minPatrolDistance, maxPatrolDistance);
            targetPosition = movingRight 
                ? startPosition + Vector3.right * patrolDistance 
                : startPosition - Vector3.right * patrolDistance;

            // Flip sprite agar menghadap arah gerakan
            spriteRenderer.flipX = !movingRight;
        }

        private IEnumerator Patrol()
        {
            while (true)
            {
                // NPC bergerak menuju target position ke kiri atau ke kanan
                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                    yield return null;
                }

                // Tentukan apakah NPC akan melanjutkan ke arah yang sama atau berbalik arah
                float chance = Random.value;
                if (chance >= continueChance)
                {
                    // Balik arah
                    movingRight = !movingRight;
                }

                SetRandomTargetPosition();

                // Tunggu sebelum bergerak lagi dengan waktu acak
                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
