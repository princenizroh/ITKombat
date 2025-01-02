using System;
using UnityEngine;

namespace ITKombat
{
    public class PlayerCombat : MonoBehaviour
    {
        public Animator animator;
        public bool isAttacking = false;
        public static PlayerCombat instance;

        private BuildingPunchCollider buildingCollider;
        private DoorPunch doorCollider;
        private ObjectClickHandler pialaCollider;
        private NewPlayerHelpText helpTextCollider;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void OnAttackButtonPressed()
        {
            HandlePunchCombo();
        }

        private void HandlePunchCombo()
        {
            if (buildingCollider != null && buildingCollider.CanActivateCanvas())
            {
                NewSoundManager.Instance.PlaySound("Button_Click", transform.position);
                buildingCollider.ActivateCanvas();

            }
            else if (doorCollider != null && doorCollider.CanActivateScene())
            {
                doorCollider.TransitionToScene();
            }
            else if (pialaCollider != null && pialaCollider.CanActivateCanvas())
            {
                NewSoundManager.Instance.PlaySound("Button_Click", transform.position);
                pialaCollider.ActivateCanvas();
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
        public void setPialaCollider(ObjectClickHandler collider)
        {
            pialaCollider = collider;
        }

        public void setFirstTextCollider(NewPlayerHelpText collider)
        {
            helpTextCollider = collider;
        }
    }
}
