using System.Collections;
using UnityEngine;

namespace ITKombat
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;

        [SerializeField]
        private NewMusicLibrary musicLibrary;
        [SerializeField]
        private AudioSource musicSource;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                // Preserve the root object of this hierarchy
                DontDestroyOnLoad(transform.root.gameObject);
            }
        }

        public void PlayMusic(string trackName, float fadeDuration = 0.5f)
        {
            AudioClip clip = musicLibrary.GetClipFromName(trackName);

            // If clip is found, start playing
            if (clip != null)
            {
                StartCoroutine(AnimateMusicCrossfade(clip, fadeDuration));
            }
            else
            {
                Debug.LogWarning($"Track named '{trackName}' not found in MusicLibrary.");
            }
        }

        IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
        {
            float percent = 0;
            while (percent < 1)
            {
                percent += Time.deltaTime / fadeDuration;
                musicSource.volume = Mathf.Lerp(1f, 0, percent);
                yield return null;
            }

            musicSource.clip = nextTrack;
            musicSource.Play();

            percent = 0;
            while (percent < 1)
            {
                percent += Time.deltaTime / fadeDuration;
                musicSource.volume = Mathf.Lerp(0, 1f, percent);
                yield return null;
            }
        }
    }
}