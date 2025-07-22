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
                return 35;
            case SkillType.FireSlash:
                return 30;
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
        Transform parent = other.transform.parent;
        if (parent == null) return;

        GameObject enemyRoot = parent.gameObject;

        if (!enemyRoot.CompareTag("Enemy")) return;

        // Nếu có tag Enemy ở GameObject cha => xử lý
        int damage = GetDamageBySkill();
        DamageManager.ApplyDamage(enemyRoot, damage);
    }

}
