using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITKombat
{
    public class AudioPlayerTest : MonoBehaviour
    {
        // Suara
        public AudioSource walkSoundScene1;
        public AudioSource walkSoundScene2;
        private AudioSource currentWalkSound;
        public AudioSource jumpSound;
        public AudioSource crouchSound;
        public AudioSource blockSound;
        public AudioSource punchSound1;
        public AudioSource punchSound2;
        public AudioSource punchSound3;
        public AudioSource punchSound4;
        public AudioSource skillSound1;
        public AudioSource skillSound2;
        public AudioSource skillSound3;

        // Player object dan tracking untuk punch
        public GameObject player;
        private bool isCrouching = false;
        private int punchStage = 0;

        // Cooldown untuk punch
        private float punchCooldown = 0.5f;
        private float lastPunchTime = -1f;

        private void Start()
        {
            SetWalkSound();
        }

        private void SetWalkSound()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "Test-Multiplayer")
            {
                currentWalkSound = walkSoundScene1;
            }
            else if (currentScene == "..")
            {
                currentWalkSound = walkSoundScene2;
            }
        }

        public void PlayWalkSound()
        {
            PlaySound(currentWalkSound);
        }

        public void PlayJumpSound()
        {
            PlaySound(jumpSound);
        }

        public void PlayCrouchSound()
        {
            isCrouching = true;
            PlaySound(crouchSound);
        }

        public void StopCrouchSound()
        {
            isCrouching = false;
        }

        public void PlayBlockSound()
        {
            PlaySound(blockSound);
        }

        public void PlayPunchSound()
        {
            if (Time.time - lastPunchTime < punchCooldown)
            {
                return;
            }

            lastPunchTime = Time.time;

            if (isCrouching)
            {
                Debug.Log("Crouch Attack");
                // Menghapus pemanggilan PlayCrouchAttackSound
                return;
            }

            punchStage++;
            if (punchStage > 4)
            {
                punchStage = 1;
            }

            switch (punchStage)
            {
                case 1:
                    PlaySound(punchSound1);
                    break;
                case 2:
                    PlaySound(punchSound1);
                    PlaySound(punchSound2);
                    break;
                case 3:
                    PlaySound(punchSound1);
                    PlaySound(punchSound2);
                    PlaySound(punchSound3);
                    break;
                case 4:
                    PlaySound(punchSound1);
                    PlaySound(punchSound2);
                    PlaySound(punchSound3);
                    PlaySound(punchSound4);
                    break;
            }
        }

        public void PlaySkill1Sound()
        {
            PlaySound(skillSound1);
        }

        public void PlaySkill2Sound()
        {
            PlaySound(skillSound2);
        }

        public void PlaySkill3Sound()
        {
            PlaySound(skillSound3);
        }

        private void PlaySound(AudioSource sound)
        {
            if (sound != null)
            {
                sound.Play();
            }
        }
    }
}
