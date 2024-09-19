using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D player;
    public float jumpForce = 5f;
    public float moveSpeed = 100;
    public float direction;
    private bool moveLeft, moveRight;
    private bool canJump = true; //jump 1x
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
        player.linearVelocity = new Vector2(direction * moveSpeed * Time.deltaTime, player.linearVelocity.y);

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
            canJump = false;
        }
    }

    public void LeftInputButtonDown()
    {
        //kiri
        moveLeft = true;
        direction = -1;
    }

    public void LeftInputButtonUp() 
    {
        moveLeft = false;
    }

    public void RightInputButtonDown()
    {
        //kanan
        moveRight = true;
        direction = 1;
    }

    public void RightInputButtonUp() 
    {
        moveRight = false;
    
    }
}
