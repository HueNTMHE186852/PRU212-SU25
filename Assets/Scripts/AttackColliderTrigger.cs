using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("🟡 Va chạm với: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            bool tookDamage = false;

            AuronPlayerController auronPlayer = collision.GetComponentInParent<AuronPlayerController>();
            if (auronPlayer != null)
            {
                auronPlayer.TakeDamage(damage);
                Debug.Log("💥 Gây " + damage + " sát thương cho AuronPlayerController");
                tookDamage = true;
            }

            Player1 player = collision.GetComponentInParent<Player1>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("💥 Gây " + damage + " sát thương cho Player1");
                tookDamage = true;
            }

            if (!tookDamage)
            {
                Debug.Log("❌ Không tìm thấy AuronPlayerController hoặc Player1 trên Player");
            }
        }
    }
}

