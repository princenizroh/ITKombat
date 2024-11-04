using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;

public class AudioSetting : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        LoadVolume();
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "SoundMultiplayer")
        {
            MusicManager.Instance.PlayMusic("Battle_1");
        }
        else if (currentScene == "Asrama")
        {
            MusicManager.Instance.PlayMusic("MarsITKombat");
        }
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicScript", volume);
    }
 
    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("AudioScript", volume);
    }
 
    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicScript", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
 
        audioMixer.GetFloat("AudioScript", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
 
    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicScript");
        sfxSlider.value = PlayerPrefs.GetFloat("AudioScript");
    }
}
