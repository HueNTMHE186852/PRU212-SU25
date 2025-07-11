[System.Serializable]
public class SharedPlayerStats
{
    public int Coins = 1000;

    public float DamageMultiplier = 1f; // 🌟 Start at 1x
    public int MaxMPBonus = 0;
    public int MaxHPBonus = 0;
    public float MoveSpeedBonus = 0f;

    public int HpLevel = 0;
    public int DamageLevel = 0;
    public int ManaLevel = 0;
    public int MoveSpeedLevel = 0;
    public static class GameStats
    {
        public static SharedPlayerStats sharedStats = new SharedPlayerStats();
    }
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
        return 100 + level * 50;
    }

    public bool TryUpgrade(string statName)
    {
        int cost = GetUpgradeCost(statName);
        if (Coins < cost) return false;

        switch (statName)
        {
            case "Damage":
                DamageMultiplier += 0.1f; // 📈 Increase 10% per level
                DamageLevel++;
                break;
            case "MaxMP":
                MaxMPBonus += 10;
                ManaLevel++;
                break;
            case "MaxHP":
                MaxHPBonus += 20;
                HpLevel++;
                break;
            case "MoveSpeed":
                MoveSpeedBonus += 0.5f;
                MoveSpeedLevel++;
                break;
        }

        Coins -= cost;
        return true;
    }
}
