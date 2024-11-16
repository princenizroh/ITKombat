using UnityEngine;

namespace ITKombat
{
    public class Button_Click : MonoBehaviour
    {
        
        public void soundButtonPause()
        {
            SoundManager.Instance.PlaySound3D("Button_Click", transform.position);
        }
    }
}
