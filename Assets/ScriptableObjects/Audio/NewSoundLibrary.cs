using UnityEngine;

namespace ITKombat
{
    [System.Serializable]
    public struct SoundEffect
    {
        public string groupID;       // Nama grup (contoh: "attack1", "skill1", "walk", dll)
        public AudioClip[] clips;    // Koleksi klip audio dalam grup
    }

    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
    public class NewSoundLibrary : ScriptableObject
    {
        [Header("Sound Effects")]
        public SoundEffect[] soundEffects; // Semua efek suara yang disimpan

        // Mengambil klip audio berdasarkan nama grup
        public AudioClip GetClipFromName(string name)
        {
            foreach (var soundEffect in soundEffects)
            {
                if (soundEffect.groupID == name)
                {
                    // Pilih secara acak dari array klip
                    return soundEffect.clips[Random.Range(0, soundEffect.clips.Length)];
                }
            }

            // Jika tidak ditemukan, kembalikan null
            Debug.LogWarning($"Sound group with name '{name}' not found in SoundLibrary!");
            return null;
        }
    }
}
