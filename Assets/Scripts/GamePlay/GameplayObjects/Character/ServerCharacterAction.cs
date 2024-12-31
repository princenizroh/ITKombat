using UnityEngine;
using System.Collections;
using Unity.Netcode;
namespace ITKombat
{
    public class ServerCharacterAction : NetworkBehaviour
    {
        
        public static ServerCharacterAction Instance;
        private FelixStateMachine meleeStateMachine;
        public Transform attackPoint;

        public int maxCombo = 4;
        public LayerMask enemyLayer;
        private int combo = 0;
        private float timeSinceLastAttack;
        // Animator
        private bool canAttack = true;
        private Animator animator;
        [SerializeField] public Collider2D hitbox;

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

        private void Update()
        {
            if (IsOwner) return;
        }

        public void OnButtonDown()
        {
            if (canAttack)
            {
                if (meleeStateMachine == null)
                {
                    Debug.LogError("FelixStateMachine is null! Ensure it's added to the GameObject.");
                    return;
                }

                if (meleeStateMachine.CurrentState == null)
                {
                    Debug.LogError("CurrentState is null! Check the state initialization in FelixStateMachine.");
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
            // Debug.Log("Berhasil Get Player Can Attack = " + CanAttack);
            return canAttack;
        }

 

        private void AttackAnimation(Collider2D[] hitEnemies, bool isBlocked)
        {
            switch (combo)
            {
                case 1:
                    // if (character.IsFacingRight)
                    // {
                    //     // Attack1_Right.Play();
                    // }
                    // else
                    // {
                    //     // Attack1_Left.Play();
                    // }
                    PlayAttackSound(1, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("Attack1");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    // Debug.Log("Attack 1 triggered");
                    break;
                case 2:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack2_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack2_Left.Play();
                    // }
                    PlayAttackSound(2, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("Attack2");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    // Debug.Log("Attack 2 triggered");
                    break;
                case 3:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack3_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack3_Left.Play();
                    // }
                    PlayAttackSound(3, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("Attack3");
                    StartCoroutine(ResetToIdleAfterTime(1f)); 
                    // Debug.Log("Attack 3 triggered");
                    break;
                case 4:
                    // if (character.IsFacingRight)
                    // {
                    //     Attack4_Right.Play();
                    // }
                    // else
                    // {
                    //     Attack4_Left.Play();
                    // }
                    PlayAttackSound(4, hitEnemies.Length > 0, isBlocked);
                    animator.SetTrigger("Attack4");
                    StartCoroutine(ResetToIdleAfterTime(1f));
                    // Debug.Log("Attack 4 triggered");
                    break;
            }
        }

        private IEnumerator ResetToIdleAfterTime(float time)
        {
            yield return new WaitForSeconds(time); 
            animator.SetTrigger("Idle"); 
            // Debug.Log("Reset to Idle after " + time + " seconds.");
        }

        // Untuk determinasi apakah attacknya kena atau tidak

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


        // Taruh hit dan miss soundnya disini
        
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

        private void PlayMissSound(int comboNumber)
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
