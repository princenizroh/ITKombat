using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData", order = 1)]
    public class PlayerScriptableObject : ScriptableObject
    {
        public string playerName;
        public AudioPlayerTest audioPlayerTest;
        public CharacterController2D1 characterController;
        public GameManagerButton gameManagerButton;
        public HealthBarTest healthBar;
        public PlayerAttackTestNope playerAttack;
        public PlayerMovement_2 playerMovement;
        public PlayerSkill playerSkill;

        // Kamu juga bisa menambahkan data lain seperti stat, kecepatan, atau atribut lainnya
        public float moveSpeed;
        public int maxHealth;
        public float attackForce;
    }
}
