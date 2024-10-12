using System;
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
        public AudioSource CroucSound;

        // Player object dan tracking untuk punch
        public GameObject player;

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
        public void PlayCrouchSound()
        {
            PlaySound(CroucSound);
        }

        public void PlayJumpSound()
        {
            PlaySound(jumpSound);
        }

        public void PlayBlockSound()
        {
            PlaySound(blockSound);
        }

        private void PlaySound(AudioSource sound)
        {
            if (sound != null)
            {
                sound.Play();
            }
        }

        internal void PlayBackgroundMusic()
        {
            throw new NotImplementedException();
        }
    }
}
