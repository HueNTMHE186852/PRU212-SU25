using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    public int damage = 10; // S? damage g�y ra

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("?? Player b? tr�ng ?�n t? AttackZone!");

            EnemyRun enemy = collision.GetComponent<EnemyRun>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
