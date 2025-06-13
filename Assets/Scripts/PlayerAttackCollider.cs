using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("?? ?ã va ch?m v?i: " + other.name);

        // L?y script EnemyRun t? cha c?a collider va ch?m
        EnemyRun enemy = other.GetComponentInParent<EnemyRun>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("? Gây damage cho enemy: " + enemy.name);
        }
    }
}
