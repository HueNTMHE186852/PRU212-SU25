using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    public enum PotionType { Health, Mana }
    public PotionType type;
    public int amount = 20;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Va cham voi Player!");

            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                if (type == PotionType.Health)
                {
                    Debug.Log("Hoi msu");
                    stats.Heal(amount);
                }
                else
                {
                    Debug.Log("Hoi mana");
                    stats.RestoreMana(amount);
                }
            }

            PotionSpawner spawner = FindObjectOfType<PotionSpawner>();
            if (spawner != null)
            {
                spawner.SpawnPotion();
            }
            else
            {
                Debug.LogWarning("PotionSpawner!");
            }

            Destroy(gameObject);
        }
    }

}
