using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballManager : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public int fireballCount = 10;
    public float spawnRadius = 0f;
    public float launchForce = 10f;
    public float fireballLifetime = 2f;

    private Transform boss;
    private List<GameObject> spawnedFireballs = new List<GameObject>();
    private int safeIndex = -1;

    private void Awake()
    {
        boss = transform; // Vì FireballManager là con của boss
    }

    public void StartFireballSequence()
    {
        GenerateSafeZone();
        SpawnFireballsAroundBoss();
    }

    void GenerateSafeZone()
    {
        safeIndex = Random.Range(0, fireballCount);
        Debug.Log($"🟩 Safe fireball index: {safeIndex}");
    }

    void SpawnFireballsAroundBoss()
    {
        Vector2 center = boss.position;
        float angleStep = 360f / fireballCount;

        for (int i = 0; i < fireballCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
            Vector2 spawnPos = center + offset;

            GameObject fb = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
            spawnedFireballs.Add(fb);
        }

        Debug.Log($"🔥 Spawned {spawnedFireballs.Count} fireballs close to boss");
    }


    public void LaunchAllFireballs()
    {
        Debug.Log("🚀 Launching all Fireballs outward from boss");

        foreach (var fb in spawnedFireballs)
        {
            if (fb != null)
            {
                Vector2 dir = (fb.transform.position - boss.position).normalized;
                Fireball fbScript = fb.GetComponent<Fireball>();
                if (fbScript != null)
                    fbScript.Launch(dir * launchForce, fireballLifetime);
            }
        }

        spawnedFireballs.Clear();
    }
}
