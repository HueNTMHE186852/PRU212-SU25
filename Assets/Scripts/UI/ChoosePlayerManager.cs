using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChoosingPlayerManager : MonoBehaviour
{
    [Header("Auron Components")]
    public Button auronButton;
    public Image auronImage;

    [Header("Helron Components")]
    public Button helronButton;
    public Image helronImage;

    [Header("Control Buttons")]
    public Button startButton;
    public Button backButton;

    [Header("Panels")]
    public GameObject chooseCharacterPanel;

    private string selectedCharacter = "";

    private void Start()
    {
        auronButton.onClick.AddListener(SelectAuron);
        helronButton.onClick.AddListener(SelectHelron);
        startButton.onClick.AddListener(StartGame);
        backButton.onClick.AddListener(CloseChooseCharacter);

        ResetHighlights();
    }

    public void SelectAuron()
    {
        selectedCharacter = "Auron";
        HighlightSelection(auronImage, helronImage);
    }

    public void SelectHelron()
    {
        selectedCharacter = "Helron";
        HighlightSelection(helronImage, auronImage);
    }

    private void HighlightSelection(Image selected, Image unselected)
    {
        unselected.color = new Color32(60, 60, 60, 255); 
        selected.color = Color.white;
    }

    private void ResetHighlights()
    {
        auronImage.color = Color.white;
        helronImage.color = Color.white;
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.LogWarning("No character!");
            return;
        }

        PlayerPrefs.SetString("SelectedCharacter", selectedCharacter);
        SceneManager.LoadScene("MapSelection");
    }

    public void CloseChooseCharacter()
    {
        chooseCharacterPanel.SetActive(false);
    }
}
