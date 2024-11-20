using UnityEngine;

namespace ITKombat
{
    public class Button_Click : MonoBehaviour
    {
        
        public void soundButtonClick()
        {
            NewSoundManager.Instance.PlaySound("Button_Click", transform.position);
        }
    }
}
