using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum ResultType { Win, Lose }

public class GameResultUI : MonoBehaviour
{
    [Header("General")]
    public ResultType resultType;

    [Header("UI Elements")]
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

    private RectTransform rect;
    private bool isVisible = false;
    private bool isAnimating = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        rect.anchoredPosition = hiddenPos;
        gameObject.SetActive(false); // Ẩn khi bắt đầu

        continueButton.onClick.AddListener(OnContinue);
        retryButton.onClick.AddListener(OnRetry);
        menuButton.onClick.AddListener(OnMenu);
    }

    void Update()
    {
        if (!isAnimating || rect == null) return;

        Vector2 target = isVisible ? shownPos : hiddenPos;
        rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, target, slideSpeed * Time.unscaledDeltaTime);

        // Tắt khi ẩn hoàn toàn
        if (!isVisible && Vector2.Distance(rect.anchoredPosition, hiddenPos) < 0.1f)
        {
            isAnimating = false;
            gameObject.SetActive(false);
        }

        // Dừng trượt khi hiện hoàn toàn
        if (isVisible && Vector2.Distance(rect.anchoredPosition, shownPos) < 0.1f)
        {
            isAnimating = false;
        }
    }

    public void Show(float playTime, int coins)
    {
        Debug.Log($"GameResultUI.Show called: {resultType}, time={playTime}, coins={coins}");
        gameObject.SetActive(true);
        isVisible = true;
        isAnimating = true;

        timeText.text = $"{playTime:F1}s";
        coinText.text = $"{coins}";
        continueButton.gameObject.SetActive(resultType == ResultType.Win);

        Time.timeScale = 0f;
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
        Debug.Log("➡️ Continue to next level");
        // TODO: Load màn tiếp theo
    }

    void OnRetry()
    {
        Hide();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnMenu()
    {
        Hide();
        SceneManager.LoadScene("MainMenu");
    }
}
