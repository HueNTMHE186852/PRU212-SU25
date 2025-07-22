using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    public GameObject healthPotionPrefab;
    public GameObject manaPotionPrefab;

    public float spawnInterval = 7f;

    public BoxCollider2D[] spawnZones;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPotion), 2f, spawnInterval);
    }

    void SpawnPotion()
    {
        if (spawnZones.Length == 0) return;

        // Chọn zone ngẫu nhiên
        BoxCollider2D zone = spawnZones[Random.Range(0, spawnZones.Length)];
        Vector2 spawnPos = GetRandomPointInBounds(zone.bounds);

        GameObject prefab = Random.value < 0.5f ? healthPotionPrefab : manaPotionPrefab;
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }
}
