using UnityEngine;
using static SharedPlayerStats;

public static class DamageManager
{
    public static void ApplyDamage(GameObject target, int damage)
    {
        if (target == null) return;

        float multiplier = GameStats.sharedStats.DamageMultiplier;
        int finalDamage = Mathf.RoundToInt(damage * multiplier);

        if (target.TryGetComponent(out EnemyRun enemy))
            enemy.TakeDamage(finalDamage);
        else if (target.TryGetComponent(out BossAI boss))
            boss.TakeDamage(finalDamage);
        else if (target.TryGetComponent(out DragonController dragon))
            dragon.TakeDamage(finalDamage);
    }
}