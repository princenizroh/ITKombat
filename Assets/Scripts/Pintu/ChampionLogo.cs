using UnityEngine;

namespace ITKombat
{
    public class ChampionLogo : MonoBehaviour
    {
        public Animator logoAnimator;

        public void TriggerSwing()
        {
            logoAnimator.SetTrigger("isSwing");
            Debug.Log("Animasi ranked logo: Swing");
        }

        public void TriggerIdle()
        {
            logoAnimator.SetTrigger("isIdle");
            Debug.Log("Animasi ranked logo: Idle");
        }
    }
}
