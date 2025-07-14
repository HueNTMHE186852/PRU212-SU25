using UnityEngine;
using System.IO;

[CreateAssetMenu(menuName = "Player/BaseStats")]
public class CharacterStatsSO : ScriptableObject
{
    public string characterName;
    public BasePlayerStats baseStats;

    public void LoadFromJson()
    {
        string path = Path.Combine(Application.persistentDataPath, characterName + "_BaseStats.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            baseStats = JsonUtility.FromJson<BasePlayerStats>(json);
        }
        else
        {
            Debug.LogWarning($"❌ JSON file for {characterName} not found at: {path}");
        }
    }

    public void SaveToJson()
    {
        string json = JsonUtility.ToJson(baseStats, true);
        string path = Path.Combine(Application.persistentDataPath, characterName + "_BaseStats.json");
        File.WriteAllText(path, json);
        Debug.Log($"💾 Saved base stats for {characterName} to {path}");
    }
}
