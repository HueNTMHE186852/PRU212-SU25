using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelection : MonoBehaviour, IPointerClickHandler
{
    [Header("Map Info")]
    public int levelIndex;             // <-- NEW: Level tương ứng
    public string sceneToLoad;

    [Header("UI")]
    public GameObject lockGo;
    public Sprite mapPreviewImage;
    [Header("Hover Effect")]
    public float scaleAmount = 1.1f;
    public float scaleSpeed = 8f;

    private bool isUnlock = false;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;

        // Gọi từ GameProgress để kiểm tra unlock
        isUnlock = GameProgress.Current.IsLevelUnlocked(levelIndex);
        UpdateLockUI();
    }

    private void UpdateLockUI()
    {
        lockGo.SetActive(!isUnlock);
    }

    public void OnHoverEnter()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale * scaleAmount));
    }

    public void OnHoverExit()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale));
    }

    private System.Collections.IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * scaleSpeed);
            yield return null;
        }
        transform.localScale = target;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isUnlock) return;
        GameProgress.Current.currentLevel = levelIndex; // Cập nhật level hiện tại
        LoadingBridge.SceneToLoad = sceneToLoad;
        LoadingBridge.BackgroundImage = mapPreviewImage;

        // Load scene trung gian
        SceneManager.LoadScene("LoadingScene");
    }
}
