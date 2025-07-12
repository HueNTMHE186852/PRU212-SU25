using System.Buffers.Text;
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
        return 10 + level * 5;
    }

    public bool TryUpgrade(string statName)
    {
        int cost = GetUpgradeCost(statName);
        if (Coins < cost) return false;

        switch (statName)
        {
            case "Damage":
                DamageMultiplier += 0.2f; // 📈 Increase 20% per level
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
        return true;
    }
}
