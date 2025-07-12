using UnityEngine;

public class Description : MonoBehaviour
{
    public Vector2 hiddenPos = new Vector2(0, 600);
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
        if (rect != null)
            rect.anchoredPosition = hiddenPos;
        gameObject.SetActive(false); // Ẩn panel khi bắt đầu
    }

    void Update()
    {
        if (!isAnimating) return;

        if (rect != null)
        {
            Vector2 target = isVisible ? shownPos : hiddenPos;
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, target, slideSpeed * Time.unscaledDeltaTime);

            // Khi đã đến vị trí ẩn, tắt panel
            if (!isVisible && Vector2.Distance(rect.anchoredPosition, hiddenPos) < 0.1f)
            {
                isAnimating = false;
                gameObject.SetActive(false);
            }
            // Khi đã đến vị trí hiện, dừng animating
            if (isVisible && Vector2.Distance(rect.anchoredPosition, shownPos) < 0.1f)
            {
                isAnimating = false;
            }
        }
    }

    public void TogglePanel()
    {
        if (!isVisible)
        {
            gameObject.SetActive(true); // Hiện panel trước khi trượt vào
        }
        isVisible = !isVisible;
        isAnimating = true;
        SetPause(isVisible);
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        isVisible = true;
        isAnimating = true;
        SetPause(true);
    }

    public void HidePanel()
    {
        isVisible = false;
        isAnimating = true;
        SetPause(false);
    }

    private void SetPause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
    }
}
