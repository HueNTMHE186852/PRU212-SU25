using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject settingsPanel;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;

    private void Start()
    {
        bool wasActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(true); 

        // Load saved settings
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        fullscreenToggle.isOn = Screen.fullScreen;

        ApplySettings();

        settingsPanel.SetActive(wasActive);
    }


    public void OpenSettings()
    {
        Debug.Log("OpenSettings called!");
        settingsPanel.SetActive(true);
    }


    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
    public void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        AudioManager.Instance.SetSFXVolume(value);
    }



    public void OnFullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void ApplySettings()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
        AudioManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
    }

}
