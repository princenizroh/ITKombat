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
        public AudioMixer audioMixer;
        public Slider musicSlider;
        public Slider sfxSlider;

        void Start()
        {
            LoadVolume();
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
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
    }
}