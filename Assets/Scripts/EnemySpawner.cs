using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public List<SpawnZone> spawnZones;  // Danh sách các vùng spawn
    public int zonesToSpawn = 5;        // Số vùng sẽ được chọn ngẫu nhiên
    public float minDistance = 8f;      // Khoảng cách tối thiểu giữa các quái
    public Transform playerTransform;  // Kéo Player vào trong Inspector
    public float avoidPlayerDistance = 5f; // Khoảng cách tối thiểu tới Player

    private void Start()
    {
        // ✅ Kiểm tra số lượng vùng có đủ không
        if (spawnZones.Count < zonesToSpawn)
        {
            Debug.LogWarning($"⚠️ Chỉ có {spawnZones.Count} vùng spawn, cần ít nhất {zonesToSpawn} để spawn đủ.");
        }

        var selectedZones = GetRandomZones(zonesToSpawn);

        Debug.Log($"🔄 Đã chọn {selectedZones.Count} vùng để spawn.");
        foreach (var zone in selectedZones)
        {
            Debug.Log($"➡️ Spawning tại vùng: {zone.zoneName}");
            SpawnEnemiesInZone(zone);
        }
    }

    List<SpawnZone> GetRandomZones(int count)
    {
        List<SpawnZone> shuffled = new List<SpawnZone>(spawnZones);

        // ✅ Fisher-Yates Shuffle
        for (int i = 0; i < shuffled.Count; i++)
        {
            int j = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        int finalCount = Mathf.Min(count, shuffled.Count);
        return shuffled.GetRange(0, finalCount);
    }

    void SpawnEnemiesInZone(SpawnZone zone)
    {
        List<Vector2> usedPositions = new List<Vector2>();
        int triesPerSpawn = 30;

        for (int i = 0; i < zone.spawnCount; i++)
        {
            bool found = false;
            Vector2 spawnPos = Vector2.zero;

            for (int t = 0; t < triesPerSpawn && !found; t++)
            {
                spawnPos = GetRandomPointInBounds(zone.area.bounds);
                found = true;

                foreach (var pos in usedPositions)
                {
                    if (Vector2.Distance(spawnPos, pos) < minDistance)
                    {
                        found = false;
                        break;
                    }
                }
            }

            if (found && playerTransform != null)
            {
                if (Vector2.Distance(spawnPos, playerTransform.position) < avoidPlayerDistance)
                {
                    found = false;
                }
                usedPositions.Add(spawnPos);
                GameObject enemyPrefab = zone.enemyTypes[Random.Range(0, zone.enemyTypes.Length)];
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }
}
