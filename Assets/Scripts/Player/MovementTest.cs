using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITKombat
{
    public class PlayerMovement : MonoBehaviour
    {
        // Referensi dan variabel
        private Rigidbody2D player;
        public Animator animator;
        public float direction;
        private bool moveLeft, moveRight;

        // Jalan
        public float moveSpeed;
        public AudioSource walkSound;

        // Lompat
        public float jumpForce;
        public float jumpCooldown;
        private bool canJump = true;
        public AudioSource jumpSound;

        // Menunduk
        public bool crouch;
        private bool isCrouching = false;
        public AudioSource crouchSound;

        // Jatuh
        public AudioSource fallSound;
        private bool isFalling = false;
        public float fallThreshold = -5f;

        // Block
        private bool isBlocking = false;
        private bool canBlock = true;
        public float blockCooldown;
        public AudioSource blockSound;

        // Punch
        private int punchStage = 0;
        public float punchCooldown = 0.5f;
        private bool canPunch = true;
        public AudioSource punchSound1;
        public AudioSource punchSound2;
        public AudioSource punchSound3;
        public AudioSource punchSound4;

        // Skill
        private int skillIndex = 0;
        public float skillCooldown = 1f;
        private bool canUseSkill = true;
        public AudioSource skillSound1;
        public AudioSource skillSound2;
        public AudioSource skillSound3;

        private PlayerControls controls;

        private void Awake()
        {
            controls = new PlayerControls();
            controls.Enable();

            controls.Player.Move.performed += info =>
            {
                direction = info.ReadValue<float>();
            };
        }

        private void Start()
        {
            player = GetComponent<Rigidbody2D>();

            if (walkSound == null)
            {
                walkSound = GetComponent<AudioSource>();
            }
        }

        private void Update()
        {
            player.velocity = new Vector2(direction * moveSpeed * Time.deltaTime, player.velocity.y);

            if (moveLeft)
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }

            if (moveRight)
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            }

            if (Mathf.Abs(player.velocity.y) < 0.1f)
            {
                canJump = true;
            }

            if (Mathf.Abs(player.velocity.x) > 0 && !walkSound.isPlaying)
            {
                walkSound.Play();
            }
            else if (Mathf.Abs(player.velocity.x) == 0 && walkSound.isPlaying)
            {
                walkSound.Stop();
            }

            if (Input.GetButtonDown("Block") && canBlock)
            {
                Block();
            }

            if (Input.GetButtonDown("Punch") && canPunch)
            {
                Punch();
            }

            if (Input.GetButtonDown("Skill") && canUseSkill)
            {
                UseSkill();
            }

            if (Input.GetButtonDown("Crouch"))
            {
                Crouch();
            }

            if (Input.GetButtonUp("Crouch"))
            {
                StandUp();
            }

            if (player.velocity.y < fallThreshold && !isFalling)
            {
                Fall();
            }
        }

        public void JumpInput()
        {
            if (canJump && Mathf.Abs(player.velocity.y) < 0.1f) 
            {
                player.velocity = new Vector2(player.velocity.x, jumpForce);
                StartCoroutine(JumpCooldown());
            }
        }

        private void Block()
        {
            animator.SetTrigger("Block");
            blockSound.Play();
            StartCoroutine(BlockCooldown());
        }

        private void Crouch()
        {
            if (!isCrouching)
            {
                isCrouching = true;
                animator.SetTrigger("Crouch");
                crouchSound.Play();
            }
        }

        private void Punch()
        {
            punchStage++;
            if (punchStage == 1)
            {
                PlayPunchStage(1);
            }
            else if (punchStage == 2)
            {
                PlayPunchStage(2);
            }
            else if (punchStage == 3)
            {
                PlayPunchStage(3);
            }
            else if (punchStage == 4)
            {
                PlayPunchStage(4);
                punchStage = 0;
            }

            StartCoroutine(PunchCooldown());
        }

        private void PlayPunchStage(int stage)
        {
            animator.SetTrigger("Punch" + stage);
            switch (stage)
            {
                case 1:
                    punchSound1.Play();
                    break;
                case 2:
                    punchSound2.Play();
                    break;
                case 3:
                    punchSound3.Play();
                    break;
                case 4:
                    punchSound4.Play();
                    break;
            }
        }

        private void UseSkill()
        {
            skillIndex++;
            if (skillIndex == 1)
            {
                PlaySkill(1);
            }
            else if (skillIndex == 2)
            {
                PlaySkill(2);
            }
            else if (skillIndex == 3)
            {
                PlaySkill(3);
                skillIndex = 0;
            }
            StartCoroutine(SkillCooldown());
        }

        private void PlaySkill(int skill)
        {
            animator.SetTrigger("Skill" + skill);
            switch (skill)
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

        private void Fall()
        {
            isFalling = true;
            animator.SetTrigger("Fall");
            fallSound.Play();
            StartCoroutine(ResetFallingStatus());
        }

        private IEnumerator JumpCooldown()
        {
            canJump = false;
            yield return new WaitForSeconds(jumpCooldown);
            canJump = true; 
        }

        private IEnumerator BlockCooldown()
        {
            canBlock = false;
            yield return new WaitForSeconds(blockCooldown);
            canBlock = true;
        }

        private IEnumerator PunchCooldown()
        {
            canPunch = false;
            yield return new WaitForSeconds(punchCooldown);
            canPunch = true;
        }

        private IEnumerator SkillCooldown()
        {
            canUseSkill = false;
            yield return new WaitForSeconds(skillCooldown);
            canUseSkill = true;
        }

        private IEnumerator ResetFallingStatus()
        {
            yield return new WaitForSeconds(1f);
            isFalling = false;
        }

        public void CrouchInputButtonDown()
        {
            crouch = true;
        }

        public void CrouchInputButtonUp()
        {
            crouch = false;
        }

        public void LeftInputButtonDown()
        {
            moveLeft = true;
            direction = -1;
        }

        public void LeftInputButtonUp()
        {
            moveLeft = false;
        }

        public void RightInputButtonDown()
        {
            moveRight = true;
            direction = 1;
        }

        public void RightInputButtonUp()
        {
            moveRight = false;
        }

        private void StandUp()
        {
            if (isCrouching)
            {
                isCrouching = false;
                animator.SetTrigger("StandUp");
            }
        }
    }
}
