using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player1 player = collision.GetComponentInParent<Player1>();
        if (player != null && !player.isDefending)
        {
            int damage = Mathf.RoundToInt(player.maxHealth * 0.2f);
            player.TakeDamage(damage);
        }
    }
}

