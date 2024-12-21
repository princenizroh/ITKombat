using UnityEngine;

namespace ITKombat
{
    public class Square : MonoBehaviour
    {
       [SerializeField] private ParticleSystem CollectParticle = null;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Collect();
            }
        }

        public void Collect()
        {
            if (CollectParticle != null)
            {
                CollectParticle.Play();
            }
            else
            {
                Debug.LogError("CollectParticle is not assigned.");
            }
        }
    }
}
