using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    public int meleeDamage = 10;
    private DarkBoss darkBoss;

    void Start()
    {
        darkBoss = GetComponentInParent<DarkBoss>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (darkBoss == null || !darkBoss.isAttacking) return;
        if (!collision.CompareTag("Player")) return;

        // Player1
        Player1 player = collision.GetComponentInParent<Player1>();
        if (player != null)
        {
            if (!player.isDefending)
            {
                player.TakeDamage(meleeDamage);
                Debug.Log("💥 Player1 trúng đánh thường: " + meleeDamage);
            }
            else
            {
                Debug.Log("🛡️ Player1 đang đỡ đòn");
            }
        }

        // AuronPlayerController
        AuronPlayerController auron = collision.GetComponentInParent<AuronPlayerController>();
        if (auron != null)
        {
            auron.TakeDamage(meleeDamage);
            Debug.Log("💥 AuronPlayerController trúng đánh thường: " + meleeDamage);
        }
    }
}
