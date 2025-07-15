// Gắn script này cho GroundSlamTrigger
using UnityEngine;

public class GroundSlamTrigger : MonoBehaviour
{
    public int damage = 15;
    private bool hasDamaged = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDamaged) return;
        if (!collision.CompareTag("Player")) return;

        hasDamaged = true;

        var auron = collision.GetComponentInParent<AuronPlayerController>();
        if (auron != null)
        {
            auron.TakeDamage(damage);
            Debug.Log("💥 Slam gây damage cho AuronPlayer");
        }

        var player = collision.GetComponentInParent<Player1>();
        if (player != null && !player.isDefending)
        {
            player.TakeDamage(damage);
            Debug.Log("💥 Slam gây damage cho Player1");
        }
    }

    public void ResetTrigger()
    {
        hasDamaged = false;
    }
}
