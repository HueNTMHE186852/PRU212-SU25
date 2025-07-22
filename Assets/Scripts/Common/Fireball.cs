using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 60;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player1 player = collision.GetComponentInParent<Player1>();
            AuronPlayerController auronPlayer = collision.GetComponentInParent<AuronPlayerController>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }
            else if (auronPlayer != null)
            {
                auronPlayer.TakeDamage(damage);
            }
        }
    }


    public void Launch(Vector2 velocity, float lifetime)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.isKinematic = false;
        rb.gravityScale = 0;
        rb.velocity = velocity;

        Destroy(gameObject, lifetime);
    }
}
