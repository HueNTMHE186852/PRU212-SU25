using UnityEngine;
using static SharedPlayerStats;

public static class DamageManager
{
       public static void ApplyDamage(GameObject target, int damage)
    {
        if (target == null)
        {
            Debug.LogWarning("⚠️ ApplyDamage: Target is null.");
            return;
        }

        // Nếu không phải tag "Enemy", bỏ qua
        if (!target.CompareTag("Enemy"))
        {
            Debug.LogWarning($"❌ Target '{target.name}' is not tagged as 'Enemy'.");
            return;
        }

        // Tính damage sau multiplier
        float multiplier = GameStats.sharedStats.DamageMultiplier;
        int finalDamage = Mathf.RoundToInt(damage * multiplier);

        // Debug thông tin
        Debug.Log($"🎯 Hit: {target.name} (child)");
        Debug.Log($"🔙 Root: {target.name}");
        Debug.Log($"🗡️ Base Damage: {damage}, 🔥 Multiplier: {multiplier}, 💥 Final Damage: {finalDamage}");

        // Gọi damage lên root component
        if (target.TryGetComponent(out EnemyRun enemy))
        {
            Debug.Log("✅ EnemyRun found");
            enemy.TakeDamage(finalDamage);
        }
        else if (target.TryGetComponent(out BossAI boss))
        {
            Debug.Log("✅ BossAI found");
            boss.TakeDamage(finalDamage);
        }
        else if (target.TryGetComponent(out DragonController dragon))
        {
            Debug.Log("✅ DragonController found");
            dragon.TakeDamage(finalDamage);
        }
        else if (target.TryGetComponent(out ForestBoss forestBoss))
        {
            Debug.Log("✅ BossForest found");
            forestBoss.TakeDamage(finalDamage);
        }
        else if (target.GetComponentInParent<DarkBoss>() is DarkBoss darkBoss)
        {
            Debug.Log("✅ DarkBoss found via parent");
            darkBoss.TakeDamage(finalDamage);
        }
        else
        {
            Debug.LogWarning($"❌ No damageable component found on {target.name}");
        }
    }

}

