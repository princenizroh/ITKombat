using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;
using System.Runtime.CompilerServices;
using UnityEngine.Audio;
using Mono.CSharp;

namespace ITKombat
{
    [RequireComponent(typeof(FelixStateMachine))]
    public class PlayerIFAttack : NetworkBehaviour 
    {
        private FelixStateMachine meleeStateMachine;

        public static PlayerIFAttack Instance;
        public int maxCombo = 4;
        public LayerMask enemyLayer;
        public int combo = 0;
        private float timeSinceLastAttack;

        private bool canAttack = true;
        // Animator
        private Animator animator;

        [SerializeField] public Collider2D hitbox;

        // VFX Right
        // [SerializeField] private ParticleSystem Attack1_Right = null;
        // [SerializeField] private ParticleSystem Attack2_Right = null;
        // [SerializeField] private ParticleSystem Attack3_Right = null;
        // [SerializeField] private ParticleSystem Attack4_Right = null;

        // VFX Left
        // [SerializeField] private ParticleSystem Attack1_Left = null;
        // [SerializeField] private ParticleSystem Attack2_Left = null;
        // [SerializeField] private ParticleSystem Attack3_Left = null;
        // [SerializeField] private ParticleSystem Attack4_Left = null;

        // Weapon state
        public bool isUsingWeapon; // Buat toggle manual di masing-masing prefab karakter menggunakan weapon atau tidak

        public CharacterStat characterStats;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

        }

        private void Start()
        {
            if (characterStats == null)
            {
                characterStats = GetComponent<CharacterStat>();
                if (characterStats == null)
                {
                    Debug.LogError("CharacterStat component is missing from this GameObject!");
                }
            }
        
            meleeStateMachine = GetComponent<FelixStateMachine>();
        }

        public void OnButtonDown()
        {
            if (canAttack)
            {
                if (meleeStateMachine == null)
                {
                    // Debug.LogError("FelixStateMachine is null! Ensure it's added to the GameObject.");
                    return;
                }

                if (meleeStateMachine.CurrentState == null)
                {
                    // Debug.LogError("CurrentState is null! Check the state initialization in FelixStateMachine.");
                    return;
                }

                if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
                {
                    meleeStateMachine.SetNextState(new GroundEntryState());
                }
            }
        }

        public bool GetCanAttack (bool CanAttack)
        {
            canAttack = CanAttack;
            return canAttack;
        }

        public void PlayAttackSound(int comboNumber, bool hitEnemies, bool isBlocked)
        {
            if (isBlocked == true)
            {
                PlayBlockedSound(comboNumber);
                return;
            }
        
            if (hitEnemies)
            {
                PlayHitSound(comboNumber);
                return;
            }
            PlayMissSound(comboNumber);
        }
        
        private void PlayHitSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("IF_Attack1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("IF_Attack2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("IF_Attack3", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("IF_Attack4", transform.position); break;
            }
        }

        public void PlayMissSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Attack_Miss1", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Attack_Miss2", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Kick_Miss", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("IF_Attack4", transform.position); break;
                
            }
        }

        private void PlayBlockedSound(int comboNumber)
        {
            switch (comboNumber)
            {
                case 1: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 2: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 3: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
                case 4: NewSoundManager.Instance.PlaySound("Block_NoWeapon_vs_NoWeapon", transform.position); break;
            }
        }
    }
}
