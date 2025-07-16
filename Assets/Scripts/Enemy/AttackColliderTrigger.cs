using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    public int fixedDamage = 30; // 👈 Gây 30 máu mỗi lần va chạm

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player1 player = collision.GetComponentInParent<Player1>();
        AuronPlayerController auronPlayer = collision.GetComponentInParent<AuronPlayerController>();

        if (auronPlayer != null)
        {
            auronPlayer.TakeDamage(fixedDamage);
        }

        if (player != null && !player.isDefending)
        {
            player.TakeDamage(fixedDamage);
        }
    }
}
