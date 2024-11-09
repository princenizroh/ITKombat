// using UnityEngine;
// using System.Collections;

// public class PlayerCombat : MonoBehaviour
// {
//     private Animator animator;
//     private int comboCount = 0;
//     private float lastPunchTime;
//     public float comboDelay = 0.5f;
//     private bool isAttacking = false;
//     private BuildingPunchCollider buildingCollider; // Referensi ke collider bangunan

//     private DoorPunch doorCollider;

//     void Start()
//     {
//         animator = GetComponent<Animator>();
//     }

//     void Update()
//     {
//         // Reset combo jika terlalu lama antara pukulan
//         if (Time.time - lastPunchTime > comboDelay)
//         {
//             ResetCombo();
//         }

//         // Cek apakah animasi pukulan saat ini sudah selesai
//         if (isAttacking && IsCurrentPunchAnimationComplete())
//         {
//             isAttacking = false; // Set isAttacking menjadi false jika animasi selesai
//         }
//     }

//     public void OnAttackButtonDown()
//     {
//         // Hanya izinkan menambah combo jika tidak sedang menyerang dan animasi saat ini telah selesai
//         if (!isAttacking && IsCurrentPunchAnimationComplete())
//         {
//             HandlePunchCombo();
//         }
//     }

//     private void HandlePunchCombo()
//     {
//         lastPunchTime = Time.time;
//         comboCount++;
//         if (comboCount > 4) comboCount = 1;

//         animator.SetInteger("comboCount", comboCount);
//         animator.SetTrigger("PunchTrigger");

//         isAttacking = true;
//         StartCoroutine(ResetPunchTrigger());

//         // Cek apakah player sedang memukul dan berkolisi dengan bangunan
//         if (buildingCollider != null && buildingCollider.CanActivateCanvas())
//         {
//             Debug.Log("Ini bisa?"); // Log ini seharusnya muncul jika semua kondisi terpenuhi
//             buildingCollider.ActivateCanvas();
//         }

//         // Cek apakah player sedang memukul dan berkolisi dengan bangunan
//         else if (doorCollider != null && doorCollider.CanActivateScene()) // Pastikan menggunakan doorCollider di sini
//         {
//             Debug.Log("Ini bisa?"); // Log ini seharusnya muncul jika semua kondisi terpenuhi
//             doorCollider.TransitionToScene(); // Pindahkan ke scene yang dituju
//         }
//     }

//     private IEnumerator ResetPunchTrigger()
//     {
//         yield return new WaitForSeconds(0.1f);
//         animator.ResetTrigger("PunchTrigger");
//     }

//     private bool IsCurrentPunchAnimationComplete()
//     {
//         AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
//         return stateInfo.normalizedTime >= 1f; // Pastikan animasi sudah selesai
//     }

//     private void ResetCombo()
//     {
//         comboCount = 0;
//         animator.SetInteger("comboCount", comboCount);
//         isAttacking = false;
//     }

//     public void SetBuildingCollider(BuildingPunchCollider collider)
//     {
//         buildingCollider = collider;
//     }

//     public void SetPunchCollider(DoorPunch collider)
//     {
//         doorCollider = collider;
//     }

// }
