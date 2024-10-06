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
        public AudioSource blockSound;
        public AudioSource skillSound1;
        public AudioSource skillSound2;
        public AudioSource skillSound3;

        // Player object dan tracking untuk punch
        public GameObject player;
        private bool isCrouching = false;
        private int punchStage = 0;

        // Cooldown untuk punch
        private float punchCooldown = 0.5f;
        private float lastPunchTime = -4f;

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

        public void StopCrouchSound()
        {
            isCrouching = false;
        }

        public void PlayBlockSound()
        {
            PlaySound(blockSound);
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
