using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player1 player = collision.GetComponentInParent<Player1>();
        AuronPlayerController auronPlayer = collision.GetComponentInParent<AuronPlayerController>();
        if(auronPlayer != null)
        {
            int damage = Mathf.RoundToInt(auronPlayer.maxHealth * 0.2f);
            auronPlayer.TakeDamage(damage);
        }
        if (player != null && !player.isDefending)
        {
            int damage = Mathf.RoundToInt(player.maxHealth * 0.2f);
            player.TakeDamage(damage);
        }
    }
}

