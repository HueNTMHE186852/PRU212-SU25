using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button PlayButton;
    public Button SettingsButton;
    public Button ExitButton;
    public Button UpgradeButton;

    [Header("Panels")]
    public GameObject SettingsPanel;
    public GameObject ChooseCharacterPanel;

    public IntroVideoManager introVideoManager;

    private bool hasPlayedIntro = false;
    public GameObject UpgradePanel;

    private void Start()
    {
        if (SettingsPanel != null)
            SettingsPanel.SetActive(false);

        if (ChooseCharacterPanel != null)
            ChooseCharacterPanel.SetActive(false);

        PlayButton.onClick.AddListener(PlayWithIntro);
        if (UpgradePanel != null)
            UpgradePanel.SetActive(false);

        UpgradeButton.onClick.AddListener(OpenUpgrade);
        PlayButton.onClick.AddListener(OpenCharacterSelection);
        SettingsButton.onClick.AddListener(OpenSettings);
        ExitButton.onClick.AddListener(QuitGame);
    }

    public void PlayWithIntro()
    {
        if (!hasPlayedIntro)
        {
            hasPlayedIntro = true;
            introVideoManager?.PlayIntro(); // Gọi trực tiếp
        }
        else
        {
            OpenCharacterSelection();
        }
    }

    public void OpenCharacterSelection()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

        if (ChooseCharacterPanel != null)
            ChooseCharacterPanel.SetActive(true);
    }

    public void CloseCharacterSelection()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

        if (UpgradePanel != null)
            UpgradePanel.SetActive(false);
    }
    public void OpenUpgrade()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

        if (UpgradePanel != null)
        {
            UpgradePanel.SetActive(true);
            ChooseCharacterPanel.SetActive(false);
        }
    }

    public void CloseUpgrade()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");
        if (UpgradePanel != null)
            UpgradePanel.SetActive(false);
    }
    public void OpenSettings()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");

        if (SettingsPanel != null)
            SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

        if (SettingsPanel != null)
            SettingsPanel.SetActive(false);
    }


    public void QuitGame()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
