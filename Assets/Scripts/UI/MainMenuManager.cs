using UnityEngine;
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
    public GameObject UpgradePanel;

    private void Start()
    {
        if (SettingsPanel != null)
            SettingsPanel.SetActive(false);

        if (ChooseCharacterPanel != null)
            ChooseCharacterPanel.SetActive(false);

        if (UpgradePanel != null)
            UpgradePanel.SetActive(false);

        UpgradeButton.onClick.AddListener(OpenUpgrade);
        PlayButton.onClick.AddListener(OpenCharacterSelection);
        SettingsButton.onClick.AddListener(OpenSettings);
        ExitButton.onClick.AddListener(QuitGame);
    }

    public void OpenCharacterSelection()
    {
        if (ChooseCharacterPanel != null)
            ChooseCharacterPanel.SetActive(true);
    }

    public void CloseCharacterSelection()
    {
        if (UpgradePanel != null)
            UpgradePanel.SetActive(false);
    }
    public void OpenUpgrade()
    {
        if (UpgradePanel != null)
        {
            UpgradePanel.SetActive(true);
            ChooseCharacterPanel.SetActive(false);
        }
    }

    public void CloseUpgrade()
    {
        if (UpgradePanel != null)
            UpgradePanel.SetActive(false);
    }
    public void OpenSettings()
    {
        if (SettingsPanel != null)
            SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (SettingsPanel != null)
            SettingsPanel.SetActive(false);
    }


    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
