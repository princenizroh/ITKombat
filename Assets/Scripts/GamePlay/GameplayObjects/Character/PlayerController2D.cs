using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Unity.Netcode;
namespace ITKombat
{
    public class PlayerController2D : NetworkBehaviour
    {
    [SerializeField] private float m_JumpForce = 250f;                          
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;            
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;    
    [SerializeField] private bool m_AirControl = false;                          
    [SerializeField] private LayerMask m_WhatIsGround;                          
    [SerializeField] private Transform m_GroundCheck;                            
    [SerializeField] private Transform m_CeilingCheck;                          
    [SerializeField] private Collider2D m_CrouchDisableCollider;                
    const float k_GroundedRadius = .2f;
    private bool m_Grounded;         
    const float k_CeilingRadius = .2f; 
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_wasCrouching = false;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    public BoolEvent OnCrouchEvent;

    [SerializeField] private SpriteRenderer spriteRenderer;
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
        if (IsOwner)
        {
            isFacingRight.OnValueChanged += OnFacingDirectionChanged;
        }
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
        if (IsOwner) 
        {
            HandleCrouching(crouch);
            HandleMovement(move);
            HandleJump(jump);
        }
    }

    private void OnFacingDirectionChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"OnFacingDirectionChanged called on client: {newValue}");
        // Update flip pada SpriteRenderer
        spriteRenderer.flipX = !newValue;
    }
    
    private void OnDestroy()
    {
        if (IsOwner)
        {
            isFacingRight.OnValueChanged -= OnFacingDirectionChanged;
        }
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
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
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

            if (IsServer){
                if (move > 0 && !isFacingRight.Value)
                {
                    isFacingRight.Value = true;
                    Debug.Log("Menghadap Kanan");
                }
                else if (move < 0 && isFacingRight.Value)
                {
                    isFacingRight.Value = false;
                    Debug.Log("Menghadap Kiri");
                }
            }
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
            m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
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

    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        // m_FacingRight = !m_FacingRight;

        // Vector3 theScale = transform.localScale;
        // theScale.x *= -1;
        // transform.localScale = theScale;
    }

    public void Dash(float dashSpeed, float dashDuration)
    {
        // float dashDirection = m_FacingRight ? 1f : -1f;
        // Vector2 dashVelocity = new(dashSpeed * dashDirection, m_Rigidbody2D.linearVelocity.y);
        // m_Rigidbody2D.linearVelocity = dashVelocity;

        // StartCoroutine(StopDashAfterDuration(dashDuration));
    }

    private IEnumerator StopDashAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
    }
    }
}
