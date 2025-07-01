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
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        int damage = GetDamageBySkill();

        EnemyRun enemy = other.GetComponentInParent<EnemyRun>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        BossAI boss = other.GetComponentInParent<BossAI>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
        }

        DragonController dragon = other.GetComponentInParent<DragonController>();
        if (dragon != null)
        {
            dragon.TakeDamage(damage);
        }
    }
}
