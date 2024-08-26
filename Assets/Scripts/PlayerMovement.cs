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
        player.velocity = new Vector2(direction * moveSpeed * Time.deltaTime, player.velocity.y);
        
        if (moveLeft)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }

        if (moveRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }

    }

    public void JumpInput()
    {
        player.velocity = new Vector2(player.velocity.x, jumpForce);
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