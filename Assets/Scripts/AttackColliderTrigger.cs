using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    public int damage = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player1 player = collision.GetComponentInParent<Player1>();
            AuronPlayerController auronPlayerController = collision.GetComponentInParent<AuronPlayerController>();
            if (auronPlayerController != null)
            {
                auronPlayerController.TakeDamage(damage);
            }
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Debug.Log("💥 Player trúng 10 dame " );
        }
    }
}
