using UnityEngine;

namespace ITKombat
{
    public class soundPlayerKimia : MonoBehaviour
    {
        public static soundPlayerKimia Instance;

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
            if (isBlocked)
            {
                PlayBlockedSound(comboNumber);
            }
            else if (hitEnemies)
            {
                PlayHitSound(comboNumber);
            }
            else
            {
                PlayMissSound(comboNumber);
            }
        }

        public void PlayHitSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Kimia_Attack1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Kimia_Attack2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Kimia_Attack3", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Kimia_Attack4", transform.position); break;
            }
        }

        public void PlayMissSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Attack_Miss1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Attack_Miss2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Attack_Miss1", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Attack_Miss_BluntWeapon1", transform.position); break;
            }
        }

        public void PlayBlockedSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_Weapon", transform.position); break;
            }
        }
    }
}
