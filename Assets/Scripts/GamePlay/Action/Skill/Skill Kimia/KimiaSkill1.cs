using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Kimia_Skill1", menuName = "Skills/Kimia/Kimia_Skill1", order = 1)]
    public class KimiaSkill1 : Skills
    {
        // Masukin sound dan anim disini
        private bool isFirstSequenceActive = false;
        private float sequenceTimer = 0f;
        public float timeToActivateSecondSequence = 5f;

        public GameObject skill1Kimia;


        public override void Activate(GameObject parent)
        {
            if (isFirstSequenceActive)
            {
                isFirstSequenceActive = true;
                // suara skill 1 tahap 1
                // NewSoundManager.Instance.PlaySound("Kimia_Skill1_Part1", parent.transform.position);
                sequenceTimer = timeToActivateSecondSequence;
                Debug.Log("Tahap pertama aktif. Lanjutkan ke tahap kedua dalam " + timeToActivateSecondSequence + " detik.");
            }
            else
            {
                Vector3 attackPoint = parent.transform.position + new Vector3(2f, 0, 0);
                Instantiate(skill1Kimia, attackPoint, Quaternion.identity);
            }
        }

        public override void BeginCooldown(GameObject parent)
        {
            //Logic cooldown skill di taruh disini
            isFirstSequenceActive = false;
            Debug.Log("Skill 1 Cooldown");
        }

        public void Update()
        {
            if (isFirstSequenceActive)
            {
                sequenceTimer -= Time.deltaTime;
                if (sequenceTimer <= 0)
                {
                    isFirstSequenceActive = false;
                    Debug.Log("Wakut habis. Tahap kedua tidak bisa dilakukan");
                }
            }
        }
    }
}
