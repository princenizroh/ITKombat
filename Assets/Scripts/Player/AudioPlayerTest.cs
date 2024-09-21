using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Tambahkan namespace ini untuk mengelola scene

namespace ITKombat
{
    public class AudioPlayerTest : MonoBehaviour
    {
        // Tombol-tombol UI untuk aksi
        public Button walkButton;
        public Button jumpButton;
        public Button crouchButton;
        public Button blockButton;
        public Button punchButton;
        public Button dashButton;
        public Button skillButton1;
        public Button skillButton2;
        public Button skillButton3;

        // Suara
        public AudioSource walkSoundScene1;
        public AudioSource walkSoundScene2;
        private AudioSource currentWalkSound; // Menyimpan suara walk yang aktif
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
        public AudioSource dashSound;

        // Player object dan tracking untuk fall
        public GameObject player;
        private bool isJumping = false;  
        private bool isGrounded = true;  

        private int punchStage = 0;  //menghitung punch

        private void Start()
        {
            walkButton.onClick.AddListener(PlayWalkSound);
            jumpButton.onClick.AddListener(PlayJumpSound);
            crouchButton.onClick.AddListener(PlayCrouchSound);
            blockButton.onClick.AddListener(PlayBlockSound);
            punchButton.onClick.AddListener(PlayPunchSound);
            dashButton.onClick.AddListener(PlayDashSound);

            skillButton1.onClick.AddListener(() => PlaySkillSound(1));
            skillButton2.onClick.AddListener(() => PlaySkillSound(2));
            skillButton3.onClick.AddListener(() => PlaySkillSound(3));

            SetWalkSound(); //set suara scene yang sekarang
        }

        private void Update()
        {
            if (isJumping && isGrounded)
            {
                PlayFallSound();
                isJumping = false; 
            }
        }

        private void SetWalkSound()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "FightTest")  //scene 1
            {
                currentWalkSound = walkSoundScene1;
            }
            else if (currentScene == "..")  //scene 2
            {
                currentWalkSound = walkSoundScene2; //kalau mau nambah tinggal copy aja
            }
        }

        private void PlayWalkSound()
        {
            if (currentWalkSound != null)
            {
                currentWalkSound.Play();
            }
        }

        private void PlayJumpSound()
        {
            if (isGrounded)  
            {
                jumpSound.Play();
                isJumping = true;
                isGrounded = false;  
            }
        }

        private void PlayCrouchSound()
        {
            crouchSound.Play();
        }

        private void PlayBlockSound()
        {
            blockSound.Play();
        }

        private void PlayPunchSound()
        {
            punchStage++;
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
            else if (punchStage == 4) //combo
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

        private void PlayFallSound()
        {
            fallSound.Play();
        }

        private void PlayDashSound()
        {
            dashSound.Play();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Ground")
            {
                isGrounded = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Ground")
            {
                isGrounded = false;
            }
        }
    }
}
