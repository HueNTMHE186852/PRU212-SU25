using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjectSpawner : MonoBehaviour
{
    public string zoneName;
    public int spawnCount = 3;
    public GameObject[] spawnPrefabs;      // Tree, Sign, etc.
    public float minDistance = 3f;
    public Transform avoidTarget;          // Optional: Player or other object
    public float avoidDistance = 5f;
    public float spawnZ = 0f;              // Z-axis position for spawn (important!)

    private Collider2D area;

    void Awake()
    {
        area = GetComponent<Collider2D>();
        zoneName = gameObject.name;
    }

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        List<Vector2> usedPositions = new List<Vector2>();
        int triesPerSpawn = 30;

        for (int i = 0; i < spawnCount; i++)
        {
            bool found = false;
            Vector2 spawnPos2D = Vector2.zero;
            GameObject prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
            float objectRadius = GetPrefabRadius(prefab);

            for (int t = 0; t < triesPerSpawn && !found; t++)
            {
                spawnPos2D = GetRandomPointInBounds(area.bounds);
                found = true;

                foreach (var pos in usedPositions)
                {
                    if (Vector2.Distance(spawnPos2D, pos) < (minDistance + objectRadius * 2))
                    {
                        found = false;
                        break;
                    }
                }

                if (avoidTarget != null && Vector2.Distance(spawnPos2D, avoidTarget.position) < avoidDistance + objectRadius)
                {
                    found = false;
                }
            }

            if (found)
            {
                usedPositions.Add(spawnPos2D);
                Vector3 spawnPos = new Vector3(spawnPos2D.x, spawnPos2D.y, spawnZ);
                Instantiate(prefab, spawnPos, Quaternion.identity);
            }
        }
    }


    Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }
    float GetPrefabRadius(GameObject prefab)
    {
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Bounds b = renderer.bounds;
            return Mathf.Max(b.extents.x, b.extents.y);
        }

        Collider2D col = prefab.GetComponentInChildren<Collider2D>();
        if (col != null)
        {
            Bounds b = col.bounds;
            return Mathf.Max(b.extents.x, b.extents.y);
        }

        return 0.5f; // Default fallback
    }
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
