using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    public enum PotionType { Health, Mana, Coin }
    public PotionType type;

    [Range(0f, 1f)]
    public float restorePercent = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player1
        var player1 = other.GetComponent<Player1>();
        if (player1 != null)
        {
            HandlePotionEffect(player1);
            Destroy(gameObject);
            return;
        }

        // Player2 (Auron)
        var player2 = other.GetComponent<AuronPlayerController>();
        if (player2 != null)
        {
            HandlePotionEffect(player2);
            Destroy(gameObject);
        }
    }

    private void HandlePotionEffect(Player1 player)
    {
        switch (type)
        {
            case PotionType.Health:
                AudioManager.Instance.PlaySFX("HPPickup");
                int healthRestore = Mathf.RoundToInt(player.maxHealth * restorePercent);
                player.currentHealth = Mathf.Min(player.maxHealth, player.currentHealth + healthRestore);
                player.healthBar.SetHealth((float)player.currentHealth / player.maxHealth);
                break;

            case PotionType.Mana:
                AudioManager.Instance.PlaySFX("MPPickup");
                int manaRestore = Mathf.RoundToInt(player.maxMP * restorePercent);
                player.currentMP = Mathf.Min(player.maxMP, player.currentMP + manaRestore);
                player.MPBar.SetMP((float)player.currentMP / player.maxMP);
                break;

            case PotionType.Coin:
                AudioManager.Instance.PlaySFX("CoinPickup");
                if (player.coinManager != null)
                {
                    player.coinManager.AddCoinOnCollect();
;
                }
                else
                {
                    Debug.LogWarning("player.coinManager dang NULL");
                }
                break;
        }
    }

    private void HandlePotionEffect(AuronPlayerController player)
    {
        switch (type)
        {
            case PotionType.Health:
                AudioManager.Instance.PlaySFX("HPPickup");
                int healthRestore = Mathf.RoundToInt(player.maxHealth * restorePercent);
                player.currentHealth = Mathf.Min(player.maxHealth, player.currentHealth + healthRestore);
                player.healthBar.SetHealth((float)player.currentHealth / player.maxHealth);
                break;

            case PotionType.Mana:
                AudioManager.Instance.PlaySFX("MPPickup");
                int manaRestore = Mathf.RoundToInt(player.maxMP * restorePercent);
                player.currentMP = Mathf.Min(player.maxMP, player.currentMP + manaRestore);
                player.MPBar.SetMP((float)player.currentMP / player.maxMP);
                break;

            case PotionType.Coin:
                AudioManager.Instance.PlaySFX("CoinPickup");
                if (player.coinManager != null)
                {
                    player.coinManager.AddCoinOnCollect();
                }
                else
                {
                    Debug.LogWarning("player.coinManager dang NULL");
                }
                break;
        }
    }
}
