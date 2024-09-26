using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D player;
    public float jumpForce = 5f;
    public float moveSpeed = 40f;
    public float direction;
    public float jumpCooldown = 2f; //Adjust jump Cooldown
    private bool canJump = true;
    private bool moveLeft, moveRight;
    public bool crouch;

    [Header("Dash Options")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float dashStartTime = 0f;

    private bool isDoubleTap = false;
    private bool isDashing = false;
    private float lastTapTime = 0f;
    private float doubleTapTime = 0.2f;

    private Rigidbody2D rb;
    public Animator animator;

    //inputSystem script
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
        // Masih on-going

 /*       if (isDashing)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontalInput + moveSpeed, rb.linearVelocityY);

            if (Time.time - lastTapTime < doubleTapTime && !isDoubleTap)
            {
                isDoubleTap = true;
                StartCoroutine(Dash());
            }
            else
            {
                lastTapTime = Time.time;
            }
        }
*/
        // Determine movement direction
        player.linearVelocity = new Vector2(direction * moveSpeed * Time.deltaTime, player.linearVelocity.y);

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

    }

/*    private IEnumerator Dash()
    {
        if (Time.time - dashStartTime > dashCooldown)
        {
            isDashing = true;
            dashStartTime = Time.time;

            // Apply dash velocity
            rb.linearVelocity = new Vector2(rb.linearVelocityX * dashSpeed, rb.linearVelocityY);

            // Adjust dash duration as needed
            yield return new WaitForSeconds(dashDuration);

            isDashing = false;
        }
    }*/

    public void JumpInput()
    {
        if (canJump)
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
}
