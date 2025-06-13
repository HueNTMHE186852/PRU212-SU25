using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    public int damage = 10; // Số damage gây ra

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("💥 Player bị trúng đòn từ AttackZone!");

            Player1 player = collision.GetComponent<Player1>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
