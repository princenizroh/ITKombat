using UnityEngine;
using UnityEngine.Audio;

namespace ITKombat
{
    public class NewSoundManager : MonoBehaviour
    {
        public static NewSoundManager Instance;

        [Header("Sound Library")]
        [SerializeField] private NewSoundLibrary soundLibrary; // Referensi ke ScriptableObject SoundLibrary

        [Header("Audio Settings")]
        [SerializeField] private AudioSource sfxSource; // AudioSource untuk memainkan suara
        [SerializeField] private AudioMixerGroup sfxMixerGroup;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            // Pastikan AudioSource menggunakan AudioMixerGroup
            if (sfxSource != null && sfxMixerGroup != null)
            {
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;
            }
        }

        public void PlaySound(string soundGroupName, Vector3 position)
        {
            AudioClip clip = soundLibrary.GetClipFromName(soundGroupName);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position);
            }
            else
            {
                Debug.LogWarning($"Sound group '{soundGroupName}' not found in SoundLibrary!");
            }
        }

        public void PlaySound2D(string soundGroupName)
        {
            AudioClip clip = soundLibrary.GetClipFromName(soundGroupName);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"Sound group '{soundGroupName}' not found in SoundLibrary!");
            }
        }
    }
}
