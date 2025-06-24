using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpikeManager : MonoBehaviour
{
    public GameObject iceSpikePrefab;
    public Transform groundReference;
    public int totalSlots = 6;
    public float delayBeforeLaunch = 1.5f;
    public float spawnRange = 50f;

    public float spikeLaunchForce = 5f;
    public float spikeLifetime = 2f;

    private List<GameObject> spawnedSpikes = new List<GameObject>();
    private HashSet<int> safeSlots = new HashSet<int>();

    public void StartSpikeAttack()
    {
        GenerateSafeZones();
        SpawnIceSpikes();
        // Không launch ngay, sẽ được gọi từ animation event
    }

    void GenerateSafeZones()
    {
        safeSlots.Clear();

        int safe1 = Random.Range(0, totalSlots);
        int safe2;
        do
        {
            safe2 = Random.Range(0, totalSlots);
        } while (safe2 == safe1);

        safeSlots.Add(safe1);
        safeSlots.Add(safe2);

        Debug.Log($"🟩 Safe Slots: {safe1}, {safe2}");
    }

    void SpawnIceSpikes()
    {
        float minX = groundReference.position.x - spawnRange / 2f;
        float slotWidth = spawnRange / totalSlots;
        int created = 0;

        for (int i = 0; i < totalSlots; i++)
        {
            if (safeSlots.Contains(i)) continue;

            float xCenter = minX + slotWidth * i + slotWidth / 2f;
            Vector2 pos = new Vector2(xCenter, groundReference.position.y);
            GameObject spike = Instantiate(iceSpikePrefab, pos, Quaternion.identity);
            spawnedSpikes.Add(spike);
            created++;
        }

        Debug.Log($"✅ Spawned {created} ice spikes (outside safe zones)");
    }

    // ✅ Gọi từ Animation Event (tại thời điểm dậm tay)
    public void LaunchAllSpikes()
    {
        Debug.Log("🚀 Launching all Ice Spikes from animation event");

        foreach (var spike in spawnedSpikes)
        {
            if (spike != null)
            {
                IceSpike spikeScript = spike.GetComponent<IceSpike>();
                if (spikeScript != null)
                    spikeScript.Launch(spikeLaunchForce, spikeLifetime);
            }
        }

        spawnedSpikes.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("🧊 Test Spawn IceSpikes (U key)");
            StartSpikeAttack();
        }
    }
}
