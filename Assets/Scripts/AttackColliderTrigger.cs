using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    public int damage = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player1 player = collision.GetComponentInParent<Player1>();
            player.TakeDamage(damage);
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(0.1f, 0.05f));
            }

            Debug.Log("💥 Player trúng 10 dame " );
        }
    }
}
