using System.IO;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameProgress
{
    public int currentLevel = 1;
    public int highestLevelUnlocked = 1;

    public List<LevelData> levels = new List<LevelData>();

    private static string SavePath => Path.Combine(Application.persistentDataPath, "game_progress.json");

    public static GameProgress Current { get; private set; } = Load();

    public static GameProgress Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<GameProgress>(json);
        }

        var progress = new GameProgress();
        progress.InitializeLevels(100); // ví dụ: 100 màn
        return progress;
    }

    public void InitializeLevels(int count)
    {
        for (int i = 0; i < count; i++)
        {
            levels.Add(new LevelData());
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("📁 GameProgress saved: " + SavePath);
    }

    public void CompleteLevel(int levelIndex, float timeTaken)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count) return;

        var level = levels[levelIndex];
        level.isCompleted = true;
        level.completionTime = Mathf.Min(level.completionTime == 0 ? float.MaxValue : level.completionTime, timeTaken); // thời gian ngắn nhất

        if (levelIndex + 1 > highestLevelUnlocked)
            highestLevelUnlocked = levelIndex + 1;

        Save();
    }

    public bool IsLevelUnlocked(int levelIndex) => levelIndex <= highestLevelUnlocked;
    public bool IsLevelCompleted(int levelIndex) => levels[levelIndex].isCompleted;
    public float GetLevelTime(int levelIndex) => levels[levelIndex].completionTime;
}
