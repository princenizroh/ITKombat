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
        // Mendapatkan nama scene saat ini
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log(currentSceneName + "adalah scene sekarang");
        if (currentSceneName == "SoundMultiplayer")
        {
            MusicManager.Instance.PlayMusic("Battle_1");
        }
        else if (currentSceneName == "Asrama")
        {
            MusicManager.Instance.PlayMusic("MarsITKombat");
        }
        // Menampilkan nama scene di console
        Debug.Log("Nama scene saat ini: " + currentSceneName);

        //Nanti tambahkan else StopMusic()
    
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
        PlayerPrefs.SetFloat("MusicScript", musicVolume);
 
        audioMixer.GetFloat("AudioScript", out float sfxVolume);
        PlayerPrefs.SetFloat("AudioScript", sfxVolume);
    }
 
    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicScript");
        sfxSlider.value = PlayerPrefs.GetFloat("AudioScript");
    }
}
