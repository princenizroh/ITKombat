using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Interactions;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 40f;
    public float direction;
    private bool moveLeft, moveRight;
    public bool crouch;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float jumpCooldown = 2f; //Adjust jump Cooldown
    private bool canJump = true;
    public float groundCheckRadius = 1f;
    private bool grounded;

    [Header("Dash Options")]
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 20f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    private float dashStartTime = 0f;
    private float dashTime;
    private float lastDashTime;
    private Vector2 dashDirection;

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Rigidbody2D player;

    //inputSystem script
    public KeyCode key;
    PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();

        controls.Player.Move.performed += info =>
        {
            direction = info.ReadValue<float>();
        };

    }

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
/*        if (isDashing)
        {
            if (Time.time >= dashingTime)
            {
                EndDash();
            }
        }*/
        // Add directional velocity
        if (moveLeft)
        {

            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }

        if (moveRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }

        if (Mathf.Abs(player.linearVelocity.y) < 0.1f)
        {
            canJump = true;
        }

        // Masih on-going

/*        controls.Player.Move.performed += ctx =>
        {
            if (ctx.interaction is MultiTapInteraction multiTap && canDash)
            {
                if (multiTap.tapCount == 2)
                    {
                        StartCoroutine(Dash());
                        Debug.Log("Double-tap successfull");
                    }
            else
                {
                    Debug.Log("Double-tap is not successfull");
                }
            }
        };*/
/*
        if (Input.GetKey(key) && canDash)
        {
            Debug.Log("Dash!");
            StartDash();
        }*/

        // Check if player in ground or not to prevent spam
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    }

    private void FixedUpdate()
    {
        player.linearVelocity = new Vector2(direction * moveSpeed * Time.deltaTime, player.linearVelocity.y);
    }

    // Mekanik dash (tinggal dicoba aja pake key keyboard dulu)
/*    private void StartDash()
    {
        isDashing = true;
        canDash = false;

        dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    
        if (dashDirection ==  Vector2.zero)
        {
            dashDirection = new Vector2(transform.localScale.x, 0);
        }

        player.linearVelocity = dashDirection * dashingPower;
        dashTime = Time.time + dashingTime;
        lastDashTime = Time.time;
    }

    private void EndDash()
    {
        isDashing = false;
        player.linearVelocity = Vector2.zero;

        Invoke(nameof(ResetDash), dashingCooldown);
    }

    private void ResetDash()
    {
        canDash = true;*/
/*    }*/

    // Mekanik jump sudah selesai
    public void JumpInput()
    {
        if (canJump && grounded)
        {
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpForce);
            // Executing Timed Jump
            StartCoroutine(JumpCooldown());

            Debug.Log("JUMPING!");
        }

    }

    IEnumerator JumpCooldown()
    {
        // Pauses Jump 
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    // Button Inputs
    public void Crouch()
    {
        crouch = true;
        Debug.Log("Crouching");
        
    }

    public void StopCrouch()
    {
        crouch = false;
        Debug.Log("Stop Crouching!");
    }

    public void MoveLeft()
    {
        // Button Left
        moveLeft = true;
        direction = -1;
    }

    public void StopMoveLeft()
    {
        moveLeft = false;
    }

    public void MoveRight()
    {
        // Button Right
        moveRight = true;
        direction = 1;
    }

    public void StopMoveRight()
    {
        moveRight = false;

    }

    // Visible ground check
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
