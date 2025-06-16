using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);

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
    }
}
