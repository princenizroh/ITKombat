using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 250f;                          // Amount of force added when the player jumps.
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;           // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    public bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_wasCrouching = false;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    public BoolEvent OnCrouchEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
    }


    private void CheckIfGrounded()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                Debug.Log("The " + gameObject.name + " is grounded");
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        HandleCrouching(crouch);
        HandleMovement(move);
        HandleJump(jump);
    }

    private void HandleCrouching(bool crouch)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }
                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }
        }
    }

    private void HandleMovement(float move)
    {
        if (m_Grounded || m_AirControl)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip(move);
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip(move);
            }
        }
    }

    private void HandleJump(bool jump)
    {
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            Debug.Log("The " + gameObject.name + " is jumping");
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    private void Flip(float move)
    {
        if (m_FacingRight && move < 0f || !m_FacingRight && move > 0f)
        {
            Debug.Log("Flipping character...");
            Debug.Log("Current facing direction: " + (m_FacingRight ? "Right" : "Left"));
            // Switch the way the player is labelled as facing.

            m_FacingRight = !m_FacingRight;
            Debug.Log("New facing direction: " + (m_FacingRight ? "Right" : "Left"));

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    public void Dash(float dashSpeed, float dashDuration)
    {
        Debug.Log("Dashing...");
        Debug.Log("Current facing direction: " + (m_FacingRight ? "Right" : "Left"));

        // Apply dash velocity
        float dashDirection = m_FacingRight ? 1f : -1f;
        Debug.Log("Dash direction: " + dashDirection);

        Vector3 dashVelocity = new Vector2(dashSpeed * dashDirection, m_Rigidbody2D.linearVelocity.y);
        m_Rigidbody2D.linearVelocity = dashVelocity;

        Flip(dashSpeed);
        // Optionally, stop the dash after a short duration
        StartCoroutine(StopDashAfterDuration(dashDuration));
    }

    private IEnumerator StopDashAfterDuration(float duration)
    {
        Debug.Log("Dash on cooldown!");
        yield return new WaitForSeconds(duration);
        // Stop dash after duration
        m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
    }
}