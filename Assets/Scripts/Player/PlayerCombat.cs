using UnityEngine;

namespace ITKombat
{
    public class PlayerCombat : MonoBehaviour
    {
        public Animator animator;
        public bool isAttacking = false;
        public static PlayerCombat instance;

        private BuildingPunchCollider buildingCollider; // Referensi ke collider bangunan
        private DoorPunch doorCollider;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        public void OnAttackButtonPressed()
        {
            HandlePunchCombo();
        }

        private void HandlePunchCombo()
        {
            // Cek apakah player sedang memukul dan berkolisi dengan bangunan
            if (buildingCollider != null && buildingCollider.CanActivateCanvas())
            {
                Debug.Log("Ini bisa?"); // Log ini seharusnya muncul jika semua kondisi terpenuhi
                buildingCollider.ActivateCanvas();
                NewSoundManager.Instance.PlaySound("Button_Click", transform.position);
            }
            else if (doorCollider != null && doorCollider.CanActivateScene()) // Pastikan menggunakan doorCollider di sini
            {
                Debug.Log("Ini bisa?"); // Log ini seharusnya muncul jika semua kondisi terpenuhi
                doorCollider.TransitionToScene(); // Pindahkan ke scene yang dituju
            }
        }

        public void SetBuildingCollider(BuildingPunchCollider collider)
        {
            buildingCollider = collider;
        }

        public void SetPunchCollider(DoorPunch collider)
        {
            doorCollider = collider;
        }
    }
}
