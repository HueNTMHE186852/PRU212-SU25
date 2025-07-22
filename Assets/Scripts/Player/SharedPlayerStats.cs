using System.IO;
using UnityEngine;

[System.Serializable]
public class SharedPlayerStats
{
    public int Coins;
    public float DamageMultiplier = 1f;

    public bool IsFirstTime = true;
    public int HpLevel;
    public int DamageLevel;
    public int ManaLevel;
    public int MoveSpeedLevel;

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
        return 20 + level * 5;
    }

    // ⬆️ Thử nâng cấp chỉ số
    public bool TryUpgrade(string statName)
    {
        int cost = GetUpgradeCost(statName);
        if (Coins < cost) return false;

        switch (statName)
        {
            case "Damage":
                if (DamageLevel >= 10) return false;
                DamageLevel++;
                DamageMultiplier += 0.2f;
                break;

            case "MaxMP":
                if (ManaLevel >= 10) return false;
                ManaLevel++;
                break;

            case "MaxHP":
                if (HpLevel >= 10) return false;
                HpLevel++;
                break;

            case "MoveSpeed":
                if (MoveSpeedLevel >= 10) return false;
                MoveSpeedLevel++;
                break;
        }

        Coins -= cost;
        SaveToJson();
        return true;
    }

}
