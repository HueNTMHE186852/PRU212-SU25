using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("UI References")]
    public GameObject canvasRoot;
    public Image backgroundImage;
    public Slider progressBar;
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI loadingText;

    private void Awake()
    {
        // Nếu đã có Instance, nhưng đang ở chính LoadingScene → chấp nhận cái mới
        if (Instance != null)
        {
            if (SceneManager.GetActiveScene().name == "LoadingScene")
            {
                Debug.Log("[LoadingManager] Replacing old instance in LoadingScene.");
                Destroy(Instance.gameObject); // Xoá bản cũ
            }
            else
            {
                Destroy(gameObject); // Giữ lại bản đã tồn tại
                return;
            }
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasRoot != null)
            DontDestroyOnLoad(canvasRoot);

        canvasRoot?.SetActive(false);
    }


    private void Start()
    {
        if (!string.IsNullOrEmpty(LoadingBridge.SceneToLoad))
        {
            LoadScene(LoadingBridge.SceneToLoad, LoadingBridge.BackgroundImage);
        }
    }

    public void LoadScene(string sceneName, Sprite background)
    {
        StartCoroutine(PrepareAndLoad(sceneName, background));
    }

    private IEnumerator PrepareAndLoad(string sceneName, Sprite background)
    {
        // Bật UI trước
        canvasRoot?.SetActive(true);

        // Cập nhật UI ngay
        progressBar.value = 0f;
        if (percentText) percentText.text = "0%";
        if (loadingText) loadingText.text = "LOADING...";
        if (backgroundImage) backgroundImage.sprite = background;

        // Đợi 1 frame để đảm bảo UI được render
        yield return null;

        // Bắt đầu load scene
        yield return StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float pct = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.value = pct;
            if (percentText) percentText.text = $"{pct * 100f:0}%";
            yield return null;
        }

        progressBar.value = 1f;
        if (percentText) percentText.text = "100%";
        yield return new WaitForSeconds(0.3f);

        op.allowSceneActivation = true;
        yield return new WaitUntil(() => op.isDone);

        yield return new WaitForSeconds(0.1f);
        canvasRoot?.SetActive(false);
    }
}
