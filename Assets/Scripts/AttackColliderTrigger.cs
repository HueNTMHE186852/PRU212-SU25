using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("🟡 Attack Collider hit: " + collision.name + " (Tag: " + collision.tag + ")");

        if (!collision.CompareTag("Player"))
            return;

        bool tookDamage = false;

        // Try AuronPlayerController
        AuronPlayerController auronPlayer = collision.GetComponentInParent<AuronPlayerController>();
        if (auronPlayer != null)
        {
            auronPlayer.TakeDamage(damage);
            Debug.Log("💥 Gây " + damage + " sát thương cho AuronPlayerController");
            tookDamage = true;
        }

        // Try Player1
        Player1 player = collision.GetComponentInParent<Player1>();
        if (player != null)
        {
            if (!player.isDefending)
            {
                player.TakeDamage(damage);
                Debug.Log("💥 Gây " + damage + " sát thương cho Player1");
                tookDamage = true;
            }
            else
            {
                Debug.Log("🛡️ Player1 is defending — no damage taken.");
            }
        }

        if (!tookDamage)
        {
            Debug.Log("❌ Không tìm thấy Player script hoặc đang phòng thủ.");
        }
    }
}
