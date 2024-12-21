using UnityEngine;

namespace ITKombat
{
    public class SwordSwing : MonoBehaviour
    {
        public Animator swordAnimator;

        public void TriggerSwing()
        {
            swordAnimator.SetTrigger("isSwing");
            Debug.Log("Animasi sword: Swing");
        }

        public void TriggerIdle()
        {
            swordAnimator.SetTrigger("isIdle");
            Debug.Log("Animasi sword: Idle");
        }
    }
}
