using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace ITKombat
{
    public class NewSoundManager : MonoBehaviour
    {
        public static NewSoundManager Instance;

        [Header("Sound Library")]
        [SerializeField] private NewSoundLibrary soundLibrary; // Referensi ke ScriptableObject SoundLibrary

        [Header("Audio Settings")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource sfx2DSource; 
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup2D;

        private bool isPlaying = false;

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

            if (sfx2DSource != null && sfxMixerGroup2D != null)
            {
                sfx2DSource.outputAudioMixerGroup = sfxMixerGroup2D;
            }
        }

        public void PlaySound(string soundGroupName, Vector3 position)
        {
            AudioClip clip = soundLibrary.GetClipFromName(soundGroupName);
            if (clip != null)
            {
                sfxSource.transform.position = position;
                sfxSource.clip = clip;
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;
                sfxSource.Play();
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
                sfx2DSource.clip = clip;
                sfx2DSource.outputAudioMixerGroup = sfxMixerGroup2D;
                sfx2DSource.Play();
            }
            else
            {
                Debug.LogWarning($"Sound group '{soundGroupName}' not found in SoundLibrary!");
            }
        }

        public void Footstep(string soundGroupName, Vector3 position)
        {
            if(!isPlaying)
            {
                AudioClip clip = soundLibrary.GetClipFromName(soundGroupName);
                if(clip != null)
                {
                    isPlaying = true;
                    sfxSource.clip = clip;
                    sfxSource.transform.position = position;
                    sfxSource.outputAudioMixerGroup = sfxMixerGroup;
                    sfxSource.Play();
                    StartCoroutine(WaitForSoundToFinish(sfxSource));
                }
                else
                {
                    Debug.LogWarning($"Sound group '{soundGroupName}' not found in SoundLibrary!");
                }
            }
        }

        private IEnumerator WaitForSoundToFinish(AudioSource audioSource)
            {
                while (audioSource != null && audioSource.isPlaying)
                {
                    yield return null;
                }
                isPlaying = false;
            }
        
    }
}
