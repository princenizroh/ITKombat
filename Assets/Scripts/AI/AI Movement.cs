using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float movementStep = 0f;
    public float maxStep = 50f;
    public bool facingPlayer = true;
    public bool canMove = true;


    [Header("Jump")]
    private float jumpForce = 5f;
    private float jumpCooldown = 2f;
    [SerializeField] private bool canJump = true;
    
    [Header("Dash")]
    // [SerializeField] private bool isDashing = false;  
    // private float dashPower = 20f;
    // private float dashTime = 0.2f;
    // private float lastTapTime = 0f;
    // private float dashTimeWindow = 0.2f;


    private Transform player;
    private Rigidbody2D myRigidbody;
    // private GameObject Ground;
    public AI_Attack aiAttack;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        aiAttack = GetComponent<AI_Attack>();
    }

    // void Update()
    // {
    //     float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    //     // Check if the AI is within attack range or currently attacking
    //     if (distanceToPlayer > aiAttack.attackRange)
    //     {
    //         if(aiAttack.currentCombo == aiAttack.maxCombo)
    //         {
    //             Retreat();  // Ensure movement stops when attacking
    //         }
    //         else 
    //         {
    //             Approach();
    //         }
    //     }
    //     else
    //     {
    //         StopMovement();
    //     }

        
    //     if (canJump){
    //         Jump();
    //     }
    // }

    public void StopMovement()
    {
        // Debug.Log("Idle State Called");
        myRigidbody.linearVelocity = Vector2.zero;
    }

    public void Approach()
    {
        // Debug.Log("Approach State Called");
        Vector2 ApproachDirection = (player.position - transform.position).normalized;

        if ((ApproachDirection.x > 0 && facingPlayer) || (ApproachDirection.x < 0 && !facingPlayer))
        {
            Flip();
        }

        myRigidbody.linearVelocity = new Vector2(ApproachDirection.x * moveSpeed, myRigidbody.linearVelocity.y);
        movementStep ++;
    }

    public void Retreat()
    {
        // Debug.Log("Retreat State Called");
        Vector2 RetreatDirection = (transform.position - player.position).normalized;

        myRigidbody.linearVelocity = new Vector2(RetreatDirection.x * moveSpeed, myRigidbody.linearVelocity.y);
        movementStep ++;
    }

    void Flip()
    {
        facingPlayer = !facingPlayer;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1; 
        transform.localScale = localScale;
    }

    void Jump()
    {
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            // Tambahkan gaya ke atas (lompatan)
            myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpForce);
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
}
