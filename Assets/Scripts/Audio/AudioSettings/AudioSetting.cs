using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;

namespace ITKombat
{
    public class AudioSetting : MonoBehaviour
    {
        public static AudioSetting Instance;
        public AudioMixer audioMixer;
        public Slider musicSlider;
        public Slider sfxSlider;

        void Awake()
        {
            // Cek jika Instance sudah ada, jika sudah hancurkan objek ini
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Menjaga objek ini tetap ada saat berpindah scene
            }
            else
            {
                Destroy(gameObject); // Hancurkan objek duplikat
            }
        }

        void Start()
        {
            LoadVolume();  // Pastikan slider diatur dengan nilai yang disimpan
        }

        public void UpdateMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", volume);
        }

        public void UpdateSoundVolume(float volume)
        {
            audioMixer.SetFloat("SFXVolume", volume);
        }

        public void SaveVolume()
        {
            audioMixer.GetFloat("MusicVolume", out float musicVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);

            audioMixer.GetFloat("SFXVolume", out float sfxVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }

        public void LoadVolume()
        {
            // Pastikan slider tidak null dan nilainya dimuat dengan benar
            if (musicSlider != null && sfxSlider != null)
            {
                musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f); // Nilai default 0.75f
                sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);     // Nilai default 0.75f
                UpdateMusicVolume(musicSlider.value); // Update volume
                UpdateSoundVolume(sfxSlider.value);  // Update volume
            }
        }
    }
}
