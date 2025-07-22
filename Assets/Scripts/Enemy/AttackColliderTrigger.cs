using UnityEngine;
using UnityEngine.SceneManagement;

public class AttackColliderTrigger : MonoBehaviour
{
    public int fixedDamage = 15; // Mặc định

    private void Start()
    {
        // Nếu đang ở Scene tên là "Level4", tăng damage lên
        if (SceneManager.GetActiveScene().name == "Level4")
        {
            fixedDamage = 30; 
        }
        // Nếu đang ở Scene tên là "Level4", giảm
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            fixedDamage = 6;
        }
    }

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
