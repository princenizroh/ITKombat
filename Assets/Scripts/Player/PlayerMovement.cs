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
    private bool isDashing = false;
    private float dashPower = 20f;
    private float dashTime = 0.2f;
    private float lastTapTime = 0f;
    private float dashTimeWindow = 0.2f;

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

            if (Mathf.Abs(direction) > 0 && Time.time - lastTapTime < dashTimeWindow && !isDashing)
            {
                Dash(new Vector2(direction, 0));
            }
            else
            {
                lastTapTime = Time.time;
            }
        };

    }

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Add directional velocity
        if (moveLeft)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            Debug.Log("Moving Left");
        }

        if (moveRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            Debug.Log("Moving Right");
        }

        if (Mathf.Abs(player.linearVelocity.y) < 0.1f)
        {
            canJump = true;
        }

        player.linearVelocity = new Vector2(direction * moveSpeed * Time.deltaTime, player.linearVelocity.y);
        // Check if player in ground or not to prevent spam
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void Dash(Vector2 direction)
    {
        if (!isDashing)
        {
            StartCoroutine(PerformDash(direction));
        }
    }

    private IEnumerator PerformDash(Vector2 direction)
    {
        isDashing = true;
        player.linearVelocity = direction * dashPower;
        yield return new WaitForSeconds(1f);
        isDashing = false;
        player.linearVelocity = Vector2.zero;
    }

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
