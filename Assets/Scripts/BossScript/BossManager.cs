using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public List<GameObject> bossPrefabs; // Gán 4 prefab boss trong Inspector
    public Transform spawnPoint; // Vị trí spawn boss
    private int currentBossIndex = 0;
    private GameObject currentBoss;

    void Start()
    {
        SpawnNextBoss();
    }

    void Update()
    {
        if (currentBoss == null)
        {
            SpawnNextBoss();
        }
    }

    void SpawnNextBoss()
    {
        if (currentBossIndex < bossPrefabs.Count)
        {
            currentBoss = Instantiate(bossPrefabs[currentBossIndex], spawnPoint.position, Quaternion.identity);
            currentBossIndex++;
        }
        else
        {
            Debug.Log("TẤT CẢ BOSS ĐÃ BỊ ĐÁNH BẠI!");
            // TODO: Hiển thị UI chiến thắng hoặc kết thúc màn chơi
        }
    }
}