using UnityEngine;

namespace ITKombat
{
    [System.Serializable]
    public struct SoundEffect
    {
        public string groupID;       
        public AudioClip[] clips;    
    }

    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
    public class NewSoundLibrary : ScriptableObject
    {
        [Header("Sound Effects")]
        public SoundEffect[] soundEffects; 

        
        public AudioClip GetClipFromName(string name)
        {
            foreach (var soundEffect in soundEffects)
            {
                if (soundEffect.groupID == name)
                {
                    return soundEffect.clips[Random.Range(0, soundEffect.clips.Length)];
                }
            }

            Debug.LogWarning($"Sound group with name '{name}' not found in SoundLibrary!");
            return null;
        }
    }
}
