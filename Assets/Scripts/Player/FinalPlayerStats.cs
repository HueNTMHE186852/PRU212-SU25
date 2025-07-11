using UnityEngine;

public class FinalPlayerStats
    {
        public int Damage;
        public int MaxMP;
        public float MoveSpeed;
        public int MaxHP;

        public void Calculate(BasePlayerStats baseStats, SharedPlayerStats upgrades)
        {
        Damage = Mathf.RoundToInt(baseStats.BaseDamage * upgrades.DamageMultiplier);
        MaxMP = baseStats.BaseMaxMP + upgrades.MaxMPBonus;
        MoveSpeed = baseStats.BaseMoveSpeed + upgrades.MoveSpeedBonus;
        MaxHP = baseStats.BaseHP + upgrades.MaxHPBonus;
        }
    }
