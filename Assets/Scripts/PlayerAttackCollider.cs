using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {

        EnemyRun enemy = other.GetComponentInParent<EnemyRun>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("? Gây damage cho enemy: " + enemy.name);
        }
    }
}
