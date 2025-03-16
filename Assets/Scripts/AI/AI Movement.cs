using System.Collections;
using UnityEngine;

namespace ITKombat
{
    public class AI_Movement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 0.1f;
        public float movementStep = 0f;
        public float timeMoving = 1.5f; 
        public float maxDistance = 2f;
        public bool facingLeft = true;
        public bool canMove = true;

        [Header("Dash")]
        public float dashSpeed = 1f;
        public float dashDuration = 0.15f;
        public float dashCooldown = 5f;
        public bool canDash = false;
        public bool isDashing = false;


        [Header("Crouch")]
        public bool isCrouching = false;
        public bool isCrouchAttacking = false;
        public float crouchCooldown = 0.5f;
        //[Header("Jump")]


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
            anim.SetTrigger("Idle");
            myRigidbody.linearVelocity = Vector2.zero;
        }

        public void Approach()
        {
            //Debug.Log("Approach State Called");
            Vector2 ApproachDirection = (player.position - transform.position).normalized;

            if ((ApproachDirection.x > 0 && facingLeft) || (ApproachDirection.x < 0 && !facingLeft))
            {
                Flip();
            }

            myRigidbody.linearVelocity = new Vector2(ApproachDirection.x * moveSpeed, myRigidbody.linearVelocity.y);
            anim.SetTrigger("Walk");
            movementStep += Time.deltaTime;
            NewSoundManager.Instance.Footstep("Walk_Floor", transform.position);
        }

        public void Retreat()
        {
            Vector2 RetreatDirection = (transform.position - player.position).normalized;

            myRigidbody.linearVelocity = new Vector2(RetreatDirection.x * moveSpeed, myRigidbody.linearVelocity.y);

            if (movementStep > 1f && ((RetreatDirection.x > 0 && facingLeft) || (RetreatDirection.x < 0 && !facingLeft)))
            {
                Flip();
            }

            anim.SetTrigger("Walk");
            movementStep += Time.deltaTime;
            NewSoundManager.Instance.Footstep("Walk_Floor", transform.position);
        }

        void Flip()
        {
            facingLeft = !facingLeft;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

        public void Dash()
        {
            if (canDash && isDashing == false)
            {
                StartCoroutine(DashRoutine());
            }
        }

        private IEnumerator DashRoutine()
        {
            isDashing = true;
            canDash = true;
            anim.SetTrigger("Dash");
            NewSoundManager.Instance.PlaySound("Dash", transform.position);

            float dashDirection = facingLeft ? -1f : 1f;
            float timer = 0f;

            while (timer < dashDuration)
            {
                transform.position += Vector3.right * (dashSpeed * dashDirection * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(dashCooldown);
            isDashing = false;
            canDash = true;
        }

        public void CrouchDown()
        {

            if (!isCrouching)
            {
                NewSoundManager.Instance.PlaySound("Crouch", transform.position);
            }
            isCrouching = true;
                anim.SetTrigger("Crouch");
                Debug.Log("AI is crouching");
        }

        public void CrouchUp()
        {
            isCrouching = false;
        }

        public void CrouchAttack()
        {
            isCrouching = true;
            Debug.Log("AI is performing a crouch attack");
            anim.SetTrigger("CrouchAttack");
            isCrouchAttacking = true;
            StartCoroutine(CrouchAttackCooldown(0.5f));
        }


        private IEnumerator CrouchAttackCooldown(float duration)
        {
            yield return new WaitForSeconds(duration);
            isCrouchAttacking = false;
        }

        public void OnCrouchAnimationEnd()
        {
            isCrouching = false; // AI can transition back to idle now
        }


        public bool IsCrouching { get { return isCrouching; } }
    }
}