using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float PlayTimeSeconds { get; private set; } = 0f;
    private bool isCountingTime = false;

    public GameResultUI winPanel;
    public GameResultUI losePanel;

    public SharedPlayerStats sharedStats = new SharedPlayerStats();

    [Header("Character Stat Assets")]
    public CharacterStatsSO auronStats;
    public CharacterStatsSO helronStats;

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
            PlayTimeSeconds += Time.deltaTime;
    }


    public void StartCountingTime()
    {
        PlayTimeSeconds = 0f;
        isCountingTime = true;
    }

    public void StopCountingTime()
    {
        isCountingTime = false;
    }
    public void ShowWin()
    {
        StopCountingTime();
        winPanel.Show(PlayTimeSeconds, 0); // 0 là số coin, bạn có thể lấy từ stats
    }

    public void ShowLose()
    {
        StopCountingTime();
        losePanel.Show(PlayTimeSeconds, 0);
    }
    public void OnBossDefeated()
    {
        LevelCompleted();
    }

    void LevelCompleted()
    {
        Debug.Log("🎉 Story Mode: Completed Level!");
        ShowWin();
        GameProgress.Current.CompleteLevel(GameProgress.Current.currentLevel, PlayTimeSeconds);
        
    }

    public void OnPlayerDead()
    {
        Debug.Log("💀 Người chơi đã chết");
        ShowLose();
    }
}
