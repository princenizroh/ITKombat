using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerAttackTest : NetworkBehaviour
{
    public Transform attackPoint;
    public float attackForce = 10f;
    public float attackRadius = 1f;
    public float attackCooldown = 0.5f; //Adjust attack cooldown
    public int maxCombo = 4;
    public float comboResetTime = 2.5f; //Adjust combo cooldown
    public LayerMask enemyLayer;
    private Animator anim;

    private int currentCombo = 0;
    private bool canAttack = true;
    private float lastAttackTime = 0f;

    PlayerControls controls;
    private InputActionAsset inputActionsAsset;
    private InputAction attackAction;

    private void Awake()
    {
        // Inisialisasi PlayerControls dan Enable
        controls = new PlayerControls();
        controls.Enable();

        // Aksi untuk serangan utama
        attackAction = controls.Player.Attack;
        attackAction.performed += ctx => Attack();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        // Cek input menggunakan Input.GetKey() untuk tombol 'P'
        if (Input.GetKey(KeyCode.P))
        {
            TestingButton();
            anim.SetTrigger("attack");
        }
    }

    public void TestingButton()
    {
        if (canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (currentCombo < maxCombo)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(transform.right * attackForce, ForceMode2D.Impulse);
                }
            }

            currentCombo++;
            lastAttackTime = Time.time;

            Debug.Log("Attack! Combo hit: " + currentCombo);

            // Combo Cooldown
            if (currentCombo == maxCombo)
            {
                StartCoroutine(ComboCooldown());
            }
            else
            {
                StartCoroutine(AttackCooldown());
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        if (Time.time - lastAttackTime > comboResetTime)
        {
            currentCombo = 0;
            Debug.Log("Combo reset due to inactivity.");
        }
    }

    private IEnumerator ComboCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(comboResetTime);

        // Reset Combo After Max
        currentCombo = 0;
        canAttack = true;

        Debug.Log("Combo reset after completing full combo.");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
