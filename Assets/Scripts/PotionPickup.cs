using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    public enum PotionType { Health, Mana }
    public PotionType type;

    [Range(0f, 1f)]
    public float restorePercent = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player1 player = other.GetComponent<Player1>();
            if (player != null)
            {
                if (type == PotionType.Health)
                {
                    int restoreAmount = Mathf.RoundToInt(player.maxHealth * restorePercent);
                    player.currentHealth = Mathf.Min(player.maxHealth, player.currentHealth + restoreAmount);
                    player.healthBar.SetHealth((float)player.currentHealth / player.maxHealth);
                }
                else if (type == PotionType.Mana)
                {
                    int restoreAmount = Mathf.RoundToInt(player.maxMP * restorePercent);
                    player.currentMP = Mathf.Min(player.maxMP, player.currentMP + restoreAmount);
                    player.MPBar.SetMP((float)player.currentMP / player.maxMP);
                }

                Destroy(gameObject);
            }
        }
    }

}
