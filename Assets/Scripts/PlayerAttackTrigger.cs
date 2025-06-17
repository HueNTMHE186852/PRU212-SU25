using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    public enum SkillType
    {
        BasicAttack,
        HeavySlash,
        FireSlash
    }

    public SkillType skillType = SkillType.BasicAttack;

    private int GetDamageBySkill()
    {
        switch (skillType)
        {
            case SkillType.BasicAttack:
                return 10;
            case SkillType.HeavySlash:
                return 50;
            case SkillType.FireSlash:
                return 50;
            default:
                return 0;
        }
    }
    public void SetSkillType(string skillName)
    {
        if (System.Enum.TryParse(skillName, out SkillType parsedSkill))
        {
            skillType = parsedSkill;
            Debug.Log("Skill type set to: " + skillType);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        int damage = GetDamageBySkill();

        EnemyRun enemy = other.GetComponentInParent<EnemyRun>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"🗡️ Gây {damage} damage cho enemy: {enemy.name} bằng skill {skillType}");
        }

        BossAI boss = other.GetComponentInParent<BossAI>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Debug.Log($"🔥 Gây {damage} damage cho boss: {boss.name} bằng skill {skillType}");
        }
    }
}
