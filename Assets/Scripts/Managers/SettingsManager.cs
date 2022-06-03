using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] AudioMixer sfxMixer;
    [SerializeField] Slider sfxSlider;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] Slider musicSlider;

    void Awake()
    {
        Application.targetFrameRate = 60;

        if (PlayerPrefs.GetInt("FirstTimeLoading") == 0)
        {
            SetEffectsVolume(1);
            SetMusicVolume(1);
            sfxSlider.value = 1;
            musicSlider.value = 1;
            PlayerPrefs.SetInt("FirstTimeLoading", 1);
            Debug.Log("First time loading");
            return;
        }

        LoadSettings();
    }


    void LoadSettings()
    {
        // GET VALUES
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume");
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume");

        // VOLUMES
        SetEffectsVolume(sfxVolume);
        SetMusicVolume(musicVolume);
        sfxSlider.value = sfxVolume;
        musicSlider.value = musicVolume;
    }

    public void SetEffectsVolume(float volume)
    {
        sfxMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SfxVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
