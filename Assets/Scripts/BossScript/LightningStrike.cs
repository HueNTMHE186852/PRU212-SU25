using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    public float damage = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player1>();
            if (player != null && !player.isDefending)
            {
                player.TakeDamage((int)damage);
                Debug.Log("⚡ Tia sét gây damage lên Player1");
            }

            var auron = collision.GetComponent<AuronPlayerController>();
            if (auron != null)
            {
                auron.TakeDamage((int)damage);
                Debug.Log("⚡ Tia sét gây damage lên AuronPlayerController");
            }
        }
    }
}
