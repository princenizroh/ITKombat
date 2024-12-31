using System.Collections;
using ITKombat;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;

public class CharacterController2D1 : NetworkBehaviour
{
    [SerializeField] private float m_JumpForce = 250f;                          
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;            
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;    
    [SerializeField] private bool m_AirControl = false;                          
    [SerializeField] private LayerMask m_WhatIsGround;                          
    [SerializeField] private Transform m_GroundCheck;                            
    [SerializeField] private Transform m_CeilingCheck;                          
    // [SerializeField] private Collider2D m_CrouchDisableCollider;
    [SerializeField] private Transform meleeHitbox;
    const float k_GroundedRadius = .2f;
    public bool m_Grounded;         
    const float k_CeilingRadius = .2f; 
    private Rigidbody2D m_Rigidbody2D;
    public bool m_FacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_wasCrouching = false;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    public BoolEvent OnCrouchEvent;

    private SpriteRenderer spriteRenderer;
    public bool IsFacingRight => m_FacingRight;
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        spriteRenderer = GetComponent<SpriteRenderer>();
        isFacingRight.Value = m_FacingRight;
        Debug.Log("isFacingRight: " + isFacingRight.Value);
        Debug.Log("m_FacingRight: " + m_FacingRight);
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
    }

    private void CheckIfGrounded()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
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
        if (!crouch)
        {
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        if (m_Grounded || m_AirControl)
        {
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }
                // if (m_CrouchDisableCollider != null)
                //     m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // if (m_CrouchDisableCollider != null)
                //     m_CrouchDisableCollider.enabled = true;

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
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
            m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            if (move > 0 && !m_FacingRight)
            {
                if(GetComponent<ServerCharacterMovement>())
                {
                    FlipServerRpc(true);
                }
                else if(GetComponent<PlayerMovement_2>())
                {
                    Flip();
                } 
            }
            else if (move < 0 && m_FacingRight)
            {
                if(GetComponent<ServerCharacterMovement>())
                {
                    FlipServerRpc(false);
                }
                else if(GetComponent<PlayerMovement_2>())
                {
                    Flip();
                } 
            }
        }
    }

    private void HandleJump(bool jump)
    {
        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FlipServerRpc(bool facingRight)
    {
        if (isFacingRight.Value != facingRight)
        {
            isFacingRight.Value = facingRight;
            FlipClientRpc(facingRight);
        }
    }

    [ClientRpc]
    public void FlipClientRpc(bool facingRight)
    {
        spriteRenderer.flipX = !facingRight;
        m_FacingRight = facingRight;
        if (meleeHitbox != null)
        {
            Vector3 hitBoxScale = meleeHitbox.localScale;
            hitBoxScale.x *= -1;  // Balik posisi X offset
            meleeHitbox.localScale = hitBoxScale;
        }
    }
    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        m_FacingRight = !m_FacingRight;
        
        if (meleeHitbox != null)
        {
            Vector3 hitBoxScale= meleeHitbox.localScale;
            hitBoxScale.x *= -1; 
            meleeHitbox.localScale = hitBoxScale;
        }
    }

    public void Dash(float dashSpeed, float dashDuration)
    {
        float dashDirection = m_FacingRight ? 1f : -1f;
        Vector2 dashVelocity = new(dashSpeed * dashDirection, m_Rigidbody2D.linearVelocity.y);
        m_Rigidbody2D.linearVelocity = dashVelocity;

        StartCoroutine(StopDashAfterDuration(dashDuration));
    }

    private IEnumerator StopDashAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
    }
}
