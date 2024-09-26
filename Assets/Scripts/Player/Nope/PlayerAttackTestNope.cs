using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAttackTestNope : NetworkBehaviour
{
    public Transform attackPoint;
    public float attackForce = 10f;
    public float attackRadius = 1f;
    public float attackCooldown = 0.5f; 
    public int maxCombo = 4;
    public LayerMask enemyLayer;

    private int combo = 0;
    public float cooldown = 0.5f; // Cooldown antara setiap serangan dalam kombo
    private float timeSinceLastAttack; // Menyimpan waktu saat terakhir kali melakukan serangan
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Update timer untuk menghitung waktu sejak serangan terakhir
        if (Time.time - timeSinceLastAttack > cooldown)
        {
            // Reset kombo jika lebih dari 0.5 detik berlalu sejak serangan terakhir
            combo = 0;
        }

        HandleMovementInput(); // Hanya menangani pergerakan, tanpa input attack
    }

    public void PerformAttack()
    {
        // Jika cooldown selesai dan kombo belum mencapai batas
        if (Time.time - timeSinceLastAttack <= cooldown && combo < maxCombo)
        {
            combo++; // Lanjut ke kombo berikutnya
        }
        else
        {
            combo = 1; // Mulai dari kombo pertama
        }

        // Catat waktu serangan terakhir
        timeSinceLastAttack = Time.time;

        // Cek hit pada musuh dalam radius serangan
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);
            }
        }

        Debug.Log("Player performed attack " + combo);
        TriggerAttackAnimation();
    }

    private void HandleMovementInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        animator.SetFloat("moveSpeed", Mathf.Abs(moveHorizontal)); 

        Vector2 movement = new Vector2(moveHorizontal, 0);
        transform.Translate(movement * Time.deltaTime);
    }

    private void TriggerAttackAnimation()
    {
        switch (combo)
        {
            case 1:
                Debug.Log("Attack 1 triggered");
                animator.SetTrigger("attack1");
                break;
            case 2:
                Debug.Log("Attack 2 triggered");
                animator.SetTrigger("attack2");
                break;
            case 3:
                Debug.Log("Attack 3 triggered");
                animator.SetTrigger("attack3");
                break;
            case 4:
                Debug.Log("Attack 4 triggered");
                animator.SetTrigger("attack4");
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
