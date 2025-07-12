using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button PlayButton;
    public Button SettingsButton;
    public Button ExitButton;

    [Header("Panels")]
    public GameObject SettingsPanel;
    public GameObject ChooseCharacterPanel;
<<<<<<< HEAD

    public IntroVideoManager introVideoManager;

    private bool hasPlayedIntro = false;
    public GameObject UpgradePanel;
=======
>>>>>>> parent of a3e0273 (Merge branch 'develop' into huentm)

    private void Start()
    {
        if (SettingsPanel != null)
            SettingsPanel.SetActive(false);

        if (ChooseCharacterPanel != null)
            ChooseCharacterPanel.SetActive(false);

<<<<<<< HEAD
        PlayButton.onClick.AddListener(PlayWithIntro);
        if (UpgradePanel != null)
            UpgradePanel.SetActive(false);

        UpgradeButton.onClick.AddListener(OpenUpgrade);
=======
>>>>>>> parent of a3e0273 (Merge branch 'develop' into huentm)
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
        if (ChooseCharacterPanel != null)
            ChooseCharacterPanel.SetActive(true);
    }

    public void CloseCharacterSelection()
    {
        if (ChooseCharacterPanel != null)
            ChooseCharacterPanel.SetActive(false);
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
