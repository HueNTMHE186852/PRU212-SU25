using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    public GameObject healthPotionPrefab;
    public GameObject manaPotionPrefab;

    [Header("Player Reference")]
    public Transform player;

    public float minSpacing = 6f;
    public float maxSpacing = 10f;

    public float firstPotionOffsetX = 2f;

    private float nextSpawnX = 0f;

    void Start()
    {
        if (player != null)
        {
            nextSpawnX = player.position.x + firstPotionOffsetX;
            SpawnPotion();
        }
        else
        {
            Debug.LogWarning("Player reference not set on PotionSpawner.");
        }
    }
    public void SpawnPotion()
    {
        if (healthPotionPrefab == null && manaPotionPrefab == null || player == null)
        {
            Debug.LogWarning("Missing references.");
            return;
        }

        // Xác định hướng nhìn của player
        float dir = player.localScale.x < 0 ? -1f : 1f;

        float offsetX = Random.Range(minSpacing, maxSpacing);
        nextSpawnX = player.position.x + dir * offsetX;

        float spawnY = 0.2f;
        Vector3 spawnPos = new Vector3(nextSpawnX, spawnY, 0f);

        GameObject prefab = (Random.value < 0.5f) ? healthPotionPrefab : manaPotionPrefab;
        GameObject potion = Instantiate(prefab, spawnPos, Quaternion.identity);

        var pickup = potion.GetComponent<PotionPickup>();
        if (pickup != null)
        {
            pickup.spawner = this;
        }
    }

    public void OnPotionCollected()
    {
        SpawnPotion();
    }
}
