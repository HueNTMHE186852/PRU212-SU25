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
        // Load saved settings (if any)
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        fullscreenToggle.isOn = Screen.fullScreen;

        ApplySettings();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OnMusicVolumeChanged(float value)
    {
        // Save and apply music volume
        PlayerPrefs.SetFloat("MusicVolume", value);
        // AudioManager.Instance.SetMusicVolume(value); // Optional
    }

    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        // AudioManager.Instance.SetSFXVolume(value); // Optional
    }

    public void OnFullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void ApplySettings()
    {
        // Apply stored settings immediately (if needed)
        Screen.fullScreen = fullscreenToggle.isOn;
    }
}
