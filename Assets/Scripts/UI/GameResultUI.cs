using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameResultUI : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform panel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI coinText;

    [Header("Buttons")]
    public Button continueButton;
    public Button retryButton;
    public Button menuButton;

    [Header("Slide Animation")]
    public Vector2 hiddenPos = new Vector2(0, 800);
    public Vector2 shownPos = new Vector2(0, 0);
    public float slideSpeed = 800f;

    private bool isVisible = false;
    private bool isAnimating = false;

    void Start()
    {
        panel.anchoredPosition = hiddenPos;
        gameObject.SetActive(false);

        continueButton.onClick.AddListener(OnContinue);
        retryButton.onClick.AddListener(OnRetry);
        menuButton.onClick.AddListener(OnMenu);
    }

    void Update()
    {
        if (!isAnimating) return;

        Vector2 target = isVisible ? shownPos : hiddenPos;
        panel.anchoredPosition = Vector2.MoveTowards(panel.anchoredPosition, target, slideSpeed * Time.unscaledDeltaTime);

        if (!isVisible && Vector2.Distance(panel.anchoredPosition, hiddenPos) < 1f)
        {
            isAnimating = false;
            gameObject.SetActive(false);
        }
        if (isVisible && Vector2.Distance(panel.anchoredPosition, shownPos) < 1f)
        {
            isAnimating = false;
        }
    }

    /// <summary>
    /// Hiện UI thắng hoặc thua
    /// </summary>
    public void Show(bool isWin, float playTime, int coins)
    {
        Time.timeScale = 0f;
        isVisible = true;
        isAnimating = true;
        gameObject.SetActive(true);
        timeText.text = $"⏱ TIME: {playTime:F1}s";
        coinText.text = $"💰 COINS: {coins}";
    }

    public void Hide()
    {
        isVisible = false;
        isAnimating = true;
        Time.timeScale = 1f;
    }

    void OnContinue()
    {
        Hide();
        // Load màn tiếp theo nếu cần
        Debug.Log("➡️ Continue to next level");
    }

    void OnRetry()
    {
        Hide();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void OnMenu()
    {
        Hide();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
