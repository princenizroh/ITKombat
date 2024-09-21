using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class AudioPlayerTest : MonoBehaviour
    {
        // Tombol Button
        public Button walkButton;
        public Button jumpButton;
        public Button crouchButton;
        public Button fallButton;
        public Button blockButton;
        public Button punchButton;
        public Button skillButton1;
        public Button skillButton2;
        public Button skillButton3;

        // Suara untuk masing-masing aksi
        public AudioSource walkSound;
        public AudioSource jumpSound;
        public AudioSource crouchSound;
        public AudioSource fallSound;
        public AudioSource blockSound;
        public AudioSource punchSound1;
        public AudioSource punchSound2;
        public AudioSource punchSound3;
        public AudioSource punchSound4;
        public AudioSource skillSound1;
        public AudioSource skillSound2;
        public AudioSource skillSound3;

        private int punchStage = 0;

        private void Start()
        {
            // Tambahkan event listener untuk setiap button
            walkButton.onClick.AddListener(PlayWalkSound);
            jumpButton.onClick.AddListener(PlayJumpSound);
            crouchButton.onClick.AddListener(PlayCrouchSound);
            fallButton.onClick.AddListener(PlayFallSound);
            blockButton.onClick.AddListener(PlayBlockSound);
            punchButton.onClick.AddListener(PlayPunchSound);

            skillButton1.onClick.AddListener(() => PlaySkillSound(1));
            skillButton2.onClick.AddListener(() => PlaySkillSound(2));
            skillButton3.onClick.AddListener(() => PlaySkillSound(3));
        }

        private void PlayWalkSound()
        {
            walkSound.Play();
        }

        private void PlayJumpSound()
        {
            jumpSound.Play();
        }

        private void PlayCrouchSound()
        {
            crouchSound.Play();
        }

        private void PlayFallSound()
        {
            fallSound.Play();
        }

        private void PlayBlockSound()
        {
            blockSound.Play();
        }

        private void PlayPunchSound()
        {
            punchStage++;
            // Reset stage jika melebihi 4
            if (punchStage > 4)
            {
                punchStage = 1;
            }

            if (punchStage == 1)
            {
                punchSound1.Play();
            }
            else if (punchStage == 2)
            {
                punchSound1.Play();
                punchSound2.Play();
            }
            else if (punchStage == 3)
            {
                punchSound1.Play();
                punchSound2.Play();
                punchSound3.Play();
            }
            else if (punchStage == 4)
            {
                punchSound1.Play();
                punchSound2.Play();
                punchSound3.Play();
                punchSound4.Play();
            }
        }

        private void PlaySkillSound(int skillNumber)
        {
            switch (skillNumber)
            {
                case 1:
                    skillSound1.Play();
                    break;
                case 2:
                    skillSound2.Play();
                    break;
                case 3:
                    skillSound3.Play();
                    break;
            }
        }
    }
}
