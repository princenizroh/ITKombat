using UnityEngine;

namespace ITKombat
{
    public class Button_Click : MonoBehaviour
    {
        
        public void soundButtonClick()
        {
            NewSoundManager.Instance.PlaySound2D("Button_Click");
        }

        public void soundBottonBooks()
        {
            NewSoundManager.Instance.PlaySound2D("Books");
        }

        public void soundSelectChar()
        {
            NewSoundManager.Instance.PlaySound2D("Select_Character");
        }
    }
}
