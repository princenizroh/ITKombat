using System.Collections;
using UnityEngine;

namespace ITKombat
{
    public class AI_Movement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 3f;
        public float movementStep = 0f;
        public float maxStep = 3f;
        public float maxDistance = 15f;
        public bool facingPlayer = true;
        public bool canMove = true;

        [Header("Jump")]
        private float jumpForce = 5f;
        private float jumpCooldown = 2f;
        [SerializeField] private bool canJump = true;

        [Header("Others")]
        [SerializeField] private Transform player;
        private Rigidbody2D myRigidbody;
        private Animator anim;

        void Start()
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            anim = GetComponent<Animator>();
        }

        public void StopMovement()
        {
            // Debug.Log("Idle State Called");
            myRigidbody.linearVelocity = Vector2.zero;
            anim.SetTrigger("Idle");
        }

        public void Approach()
        {
            //Debug.Log("Approach State Called");
            Vector2 ApproachDirection = (player.position - transform.position).normalized;

            if ((ApproachDirection.x > 0 && facingPlayer) || (ApproachDirection.x < 0 && !facingPlayer))
            {
                Flip();
            }

            myRigidbody.linearVelocity = new Vector2(ApproachDirection.x * moveSpeed, myRigidbody.linearVelocity.y);
            anim.SetTrigger("Walk");
            movementStep += Time.deltaTime;
            // SoundManager.Instance.PlaySound3D("WalkFloor", transform.position);
        }

        public void Retreat()
        {
            Vector2 RetreatDirection = (transform.position - player.position).normalized;

            myRigidbody.linearVelocity = new Vector2(RetreatDirection.x * moveSpeed, myRigidbody.linearVelocity.y);
            anim.SetTrigger("Walk");
            movementStep += Time.deltaTime;
            // SoundManager.Instance.PlaySound3D("WalkFloor", transform.position);
        }

        void Flip()
        {
            facingPlayer = !facingPlayer;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

        void Jump()
        {
            if (canJump && Input.GetKeyDown(KeyCode.Space))
            {
                // Tambahkan gaya ke atas (lompatan)
                myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpForce);
                StartCoroutine(JumpCooldown());
                anim.SetTrigger("Jump");
                SoundManager.Instance.PlaySound3D("Jump", transform.position);
            }
        }

        private IEnumerator JumpCooldown()
        {
            canJump = false;
            yield return new WaitForSeconds(jumpCooldown);
            canJump = true;
        }
    }
}