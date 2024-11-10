using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;
    public AudioMixerGroup sfxMixerGroup; // Tambahkan ini

    private AudioSource currentAudioSource;
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
            DontDestroyOnLoad(transform.root.gameObject);
        }

        // Pastikan sfx2DSource menggunakan AudioMixerGroup
        if (sfx2DSource != null && sfxMixerGroup != null)
        {
            sfx2DSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        if(!isPlaying)
        {
            AudioClip clip = sfxLibrary.GetClipFromName(soundName);
            if (clip != null)
            {
                isPlaying = true;
                GameObject audioSourceObject = new GameObject("TempAudio");
                currentAudioSource = audioSourceObject.AddComponent<AudioSource>();
                currentAudioSource.clip = clip;
                currentAudioSource.outputAudioMixerGroup = sfxMixerGroup; // Atur group mixer
                currentAudioSource.transform.position = pos;
                currentAudioSource.Play();
                StartCoroutine(WaitForSoundToFinish(currentAudioSource));
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
        Destroy(audioSource.gameObject);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName));
    }
}