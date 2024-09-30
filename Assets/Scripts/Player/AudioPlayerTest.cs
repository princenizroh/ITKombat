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
        public AudioSource crouchAttackSound; // Suara serangan merunduk
        public AudioSource fallSound;
        public AudioSource blockSound;
        public AudioSource punchSound1;
        public AudioSource punchSound2;
        public AudioSource punchSound3;
        public AudioSource punchSound4;
        public AudioSource skillSound1;
        public AudioSource skillSound2;
        public AudioSource skillSound3;
        public AudioSource dashSound;

        // Player object dan tracking untuk fall
        public GameObject player;
        private bool isJumping = false;  
        private bool isGrounded = true;  
        private int punchStage = 0;  //menghitung punch

        // Cooldown
        private float punchCooldown = 0.5f;
        private float lastPunchTime = -1f; // Inisialisasi dengan waktu negatif

        private void Start()
        {
            SetWalkSound();
        }

        private void Update()
        {
            // Cek apakah pemain masih di tanah menggunakan Raycast
            isGrounded = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f);

            // Mainkan suara jatuh jika pemain tidak di tanah dan sedang jatuh
            if (!isGrounded && isJumping)
            {
                PlayFallSound();
                isJumping = false; 
            }
        }

        private void SetWalkSound()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "Test-Multiplayer")  //scene 1
            {
                currentWalkSound = walkSoundScene1;
            }
            else if (currentScene == "..")  //scene 2
            {
                currentWalkSound = walkSoundScene2;
            }
        }

        // Fungsi untuk memutar suara sesuai tombol
        public void PlayWalkSound()
        {
            PlaySound(currentWalkSound);
        }

        public void PlayJumpSound()
        {
            if (isGrounded)  
            {
                PlaySound(jumpSound);
                isJumping = true;
                isGrounded = false;  
            }
        }

        public void PlayCrouchSound()
        {
            PlaySound(crouchSound);
        }

        public void PlayCrouchAttackSound()
        {
            PlaySound(crouchAttackSound);
        }

        public void PlayBlockSound()
        {
            PlaySound(blockSound);
        }

        public void PlayPunchSound()
        {
            // Cek cooldown
            if (Time.time - lastPunchTime < punchCooldown)
            {
                return; // Jika masih dalam cooldown, jangan lakukan apa-apa
            }

            lastPunchTime = Time.time; // Set waktu terakhir punch

            punchStage++;
            if (punchStage > 4)
            {
                punchStage = 1; // Reset stage punch jika lebih dari 4
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
                case 4: //combo
                    PlaySound(punchSound1);
                    PlaySound(punchSound2);
                    PlaySound(punchSound3);
                    PlaySound(punchSound4);
                    break;
            }
        }

        public void PlaySkillSound(int skillNumber)
        {
            switch (skillNumber)
            {
                case 1:
                    PlaySound(skillSound1);
                    break;
                case 2:
                    PlaySound(skillSound2);
                    break;
                case 3:
                    PlaySound(skillSound3);
                    break;
            }
        }

        private void PlayFallSound()
        {
            PlaySound(fallSound);
        }

        public void PlayDashSound()
        {
            PlaySound(dashSound);
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
