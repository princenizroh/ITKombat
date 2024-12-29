using UnityEngine;

namespace ITKombat
{
    public class soundPlayerIF : MonoBehaviour
    {
        public static soundPlayerIF Instance;

        public void Awake()
        {
            
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

        }
        
        public void PlayAttackSound(int comboNumber, bool hitEnemies, bool isBlocked)
        {
            if (isBlocked == true)
            {
                PlayBlockedSound(comboNumber);
                return;
            }
        
            if (hitEnemies)
            {
                PlayHitSound(comboNumber);
                return;
            }
            PlayMissSound(comboNumber);
        }
        
        public void PlayHitSound(int comboNumber)
        {
            Debug.Log("Hit sound terpanggil.");
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("IF_Attack1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("IF_Attack2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("IF_Attack3", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("IF_Attack4", transform.position); break;
            }
        }

        public void PlayMissSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Attack_Miss1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Attack_Miss2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Kick_Miss", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("IF_Attack4", transform.position); break;
                
            }
        }

        public void PlayBlockedSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
            }
        }

    }
}
