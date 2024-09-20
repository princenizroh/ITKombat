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

        // Handle Keyboard Input

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 moveDirection = 

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
    public void CrouchInputButtonDown()
    {
        crouch = true;
        Debug.Log("Crouching");
        
    }

    public void CrouchInputButtonUp()
    {
        crouch = false;
        Debug.Log("Stop Crouching!");
    }

    public void LeftInputButtonDown()
    {
        // Button Left
        moveLeft = true;
        direction = -1;
    }

    public void LeftInputButtonUp()
    {
        moveLeft = false;
    }

    public void RightInputButtonDown()
    {
        // Button Right
        moveRight = true;
        direction = 1;
    }

    public void RightInputButtonUp()
    {
        moveRight = false;

    }
}
