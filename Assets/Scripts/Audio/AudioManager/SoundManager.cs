using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;

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
            // Use the root object to avoid the DontDestroyOnLoad error
            DontDestroyOnLoad(transform.root.gameObject);
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        if (!isPlaying)
        {
            AudioClip clip = sfxLibrary.GetClipFromName(soundName);
            if (clip != null)
            {
                isPlaying = true;

                GameObject audioSourceObject = new GameObject("TempAudio");
                currentAudioSource = audioSourceObject.AddComponent<AudioSource>();
                currentAudioSource.clip = clip;

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

    internal void PlaySound3D(object position)
    {
        throw new NotImplementedException();
    }
}
