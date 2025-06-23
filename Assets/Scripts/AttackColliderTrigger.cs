using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("🟡 Va chạm với: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            Player1 player = collision.GetComponentInParent<Player1>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("💥 Gây " + damage + " sát thương cho Player");
            }
            else
            {
                Debug.Log("❌ Không tìm thấy Player1");
            }
        }
    }
}

