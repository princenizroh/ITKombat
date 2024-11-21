using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "SoundCharacter", menuName = "Audio/SoundCharacter")]
    public class SoundCharacter : ScriptableObject
    {
        [Header("Character Info")]
        public string characterName; // Nama karakter

        [Header("Attack Sounds")]
        public AudioClip attack1;    // Suara Attack 1
        public AudioClip attack2;    // Suara Attack 2
        public AudioClip attack3;    // Suara Attack 3
        public AudioClip attack4;    // Suara Attack 4
        public AudioClip attackMiss;

        [Header("Skill Sounds")]
        public AudioClip skill1;     // Suara Skill 1
        public AudioClip skill2;     // Suara Skill 2
        public AudioClip skill3;
    }
}
