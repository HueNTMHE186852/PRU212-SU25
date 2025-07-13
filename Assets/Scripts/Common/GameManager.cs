using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float PlayTimeSeconds { get; private set; } = 0f; // ⏱️ Thời gian chơi

    private bool isCountingTime = false;

    public SharedPlayerStats sharedStats = new SharedPlayerStats();

    [Header("Character Stat Assets")]
    public CharacterStatsSO auronStats;
    public CharacterStatsSO helronStats;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SharedPlayerStats.GameStats.sharedStats = SharedPlayerStats.LoadFromJson();
            auronStats.LoadFromJson();
            helronStats.LoadFromJson();
            GameProgress.Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isCountingTime)
        {
            PlayTimeSeconds += Time.deltaTime;
        }
    }

    public void StartCountingTime()
    {
        PlayTimeSeconds = 0f;
        isCountingTime = true;
    }

    public void StopCountingTime()
    {
        isCountingTime = false;
        Debug.Log("⏳ Tổng thời gian chơi: " + PlayTimeSeconds + " giây");
    }

    public void ShowWin()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowLose()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        // Load màn tiếp theo hoặc về menu
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        // Load lại màn chơi hiện tại
    }

    public void OnBossDefeated()
    {
        LevelCompleted();
    }

    void LevelCompleted()
    {
        Debug.Log("🎉 Story Mode: Completed Level!");
        ShowWin();
        GameProgress.Current.CompleteLevel(GameProgress.Current.currentLevel, PlayTimeSeconds); // nếu bạn có logic lưu tiến trình
    }

    public void OnPlayerDead()
    {
        Debug.Log("💀 Người chơi đã chết");
        ShowLose();
    }
}
