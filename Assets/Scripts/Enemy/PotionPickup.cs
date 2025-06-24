using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    public enum PotionType { Health, Mana }
    public PotionType type;

    [Range(0f, 1f)]
    public float restorePercent = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player1
        var player1 = other.GetComponent<Player1>();
        if (player1 != null)
        {
            if (type == PotionType.Health)
            {
                int restoreAmount = Mathf.RoundToInt(player1.maxHealth * restorePercent);
                player1.currentHealth = Mathf.Min(player1.maxHealth, player1.currentHealth + restoreAmount);
                player1.healthBar.SetHealth((float)player1.currentHealth / player1.maxHealth);
            }
            else if (type == PotionType.Mana)
            {
                int restoreAmount = Mathf.RoundToInt(player1.maxMP * restorePercent);
                player1.currentMP = Mathf.Min(player1.maxMP, player1.currentMP + restoreAmount);
                player1.MPBar.SetMP((float)player1.currentMP / player1.maxMP);
            }

            Destroy(gameObject);
            return;
        }

        // Player2 (Auron)
        var player2 = other.GetComponent<AuronPlayerController>();
        if (player2 != null)
        {
            if (type == PotionType.Health)
            {
                int restoreAmount = Mathf.RoundToInt(player2.maxHealth * restorePercent);
                player2.currentHealth = Mathf.Min(player2.maxHealth, player2.currentHealth + restoreAmount);
                player2.healthBar.SetHealth((float)player2.currentHealth / player2.maxHealth);
            }
            else if (type == PotionType.Mana)
            {
                int restoreAmount = Mathf.RoundToInt(player2.maxMP * restorePercent);
                player2.currentMP = Mathf.Min(player2.maxMP, player2.currentMP + restoreAmount);
                player2.MPBar.SetMP((float)player2.currentMP / player2.maxMP);
            }

            Destroy(gameObject);
            return;
        }
    }


}
