using System.Collections;
using UnityEngine;

namespace ITKombat
{
    public class AI_Movement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 0.5f;
        public float movementStep = 0f;
        public float timeMoving = 1.5f; 
        public float maxDistance = 2f;
        public bool facingLeft = true;
        public bool canMove = true;

        [Header("Dash")]
        public float dashSpeed = 1f;
        public float dashDuration = 0.15f;
        public float dashCooldown = 5f;
        private bool canDash = true;
        private bool isDashing = false;


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

            if (movementStep > 1f &&((RetreatDirection.x > 0 && facingLeft) || (RetreatDirection.x < 0 && !facingLeft)))
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
            canDash = false;
            anim.SetTrigger("Dash");
            NewSoundManager.Instance.PlaySound("Dash", transform.position);

            float dashDirection;
            if (facingLeft){
                dashDirection = -1f;
            }
            else{
                dashDirection = 1f;
            }

            float timer = 0f;

            while (timer < dashDuration)
            {
                transform.Translate(Vector3.right * (dashSpeed * dashDirection) * Time.deltaTime);
                timer += Time.deltaTime;
            }

            yield return new WaitForSeconds(dashDuration);

            isDashing = false;

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}