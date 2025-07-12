using UnityEngine;

public class FinalPlayerStats
    {
        public int Damage;
        public int MaxMP;
        public float MoveSpeed;
        public int MaxHP;

    public void Calculate(BasePlayerStats baseStats, SharedPlayerStats upgrades)
    {
        float totalDamageMultiplier = 1f + upgrades.DamageLevel * 0.2f;
        Damage = Mathf.RoundToInt(baseStats.BaseDamage * totalDamageMultiplier);

        MaxMP = Mathf.RoundToInt(baseStats.BaseMaxMP * (1f + upgrades.ManaLevel * 0.2f));

        MoveSpeed = baseStats.BaseMoveSpeed + upgrades.MoveSpeedLevel * 0.5f;

        MaxHP = Mathf.RoundToInt(baseStats.BaseHP * (1f + upgrades.HpLevel * 0.2f));
    }

    

}
