using UnityEngine;

public class IceSpike : MonoBehaviour
{
    public int damage = 80;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player1 player = collision.GetComponentInParent<Player1>();
            player.TakeDamage(damage);
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.05f));
            }
            Debug.Log("💥 Player trúng đòn special attack trừ 80 dame ");
        }
    }
    public void Launch(float launchForce, float lifetime)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.isKinematic = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.up * launchForce * 1.5f;

        Destroy(gameObject, lifetime);
    }
}
