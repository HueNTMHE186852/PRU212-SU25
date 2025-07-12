using System.IO;
using UnityEngine;

[System.Serializable]
public class SharedPlayerStats
{
    public int Coins = 1000;
    public float DamageMultiplier = 1f;

    public int HpLevel = 0;
    public int DamageLevel = 0;
    public int ManaLevel = 0;
    public int MoveSpeedLevel = 0;

    // 🔁 Đường dẫn lưu file JSON
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player_stats.json");

    // 🌐 Singleton truy cập toàn cục
    public static class GameStats
    {
        public static SharedPlayerStats sharedStats = LoadFromJson();
    }

    // 💾 Lưu vào file JSON
    public void SaveToJson()
    {
        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("📁 Saved stats to: " + SavePath);
    }

    // 📤 Tải từ file JSON
    public static SharedPlayerStats LoadFromJson()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            Debug.Log("📂 Loaded stats from: " + SavePath);
            return JsonUtility.FromJson<SharedPlayerStats>(json);
        }

        Debug.Log("⚠️ No save file found. Creating new stats.");
        return new SharedPlayerStats(); // Mặc định nếu chưa có file
    }

    // 📊 Hàm nâng cấp
    public int GetUpgradeCost(string statName)
    {
        int level = statName switch
        {
            "Damage" => DamageLevel,
            "MaxMP" => ManaLevel,
            "MaxHP" => HpLevel,
            "MoveSpeed" => MoveSpeedLevel,
            _ => 0
        };
        return 10 + level * 5;
    }

    // ⬆️ Thử nâng cấp chỉ số
    public bool TryUpgrade(string statName)
    {
        int cost = GetUpgradeCost(statName);
        if (Coins < cost) return false;

        switch (statName)
        {
            case "Damage":
                DamageMultiplier += 0.2f;
                DamageLevel++;
                break;
            case "MaxMP":
                ManaLevel++;
                break;
            case "MaxHP":
                HpLevel++;
                break;
            case "MoveSpeed":
                MoveSpeedLevel++;
                break;
        }

        Coins -= cost;

        SaveToJson(); // 🔐 Lưu ngay sau khi nâng cấp
        return true;
    }
}
