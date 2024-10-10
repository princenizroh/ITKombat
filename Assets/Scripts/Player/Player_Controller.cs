using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    public class Player_Controller : MonoBehaviour
    {
        private Rigidbody2D m_RigidBody;

        [SerializeField]
        private float movementSpeed;
        [SerializeField]
        private float forceJump;
        private bool facingRight;
        private bool isJumping;
        private bool isGrounded;

        private void Awake(){
            m_RigidBody = GetComponent<Rigidbody2D>();
        }
        void Start()
        {
            facingRight = true;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void FixedUpdate(){
            float horizontal = Input.GetAxis(Axis.Horizontal_Axis);
            HandleMovement(horizontal);
            flip(horizontal);
        }

        private void HandleMovement(float horizontal){
            m_RigidBody.linearVelocity = new Vector2(horizontal * movementSpeed, m_RigidBody.linearVelocity.y);
        }
        
        private void flip(float horizontal){
            if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight){
                facingRight = !facingRight;
                
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }
        }
    }
}
