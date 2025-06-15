using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    public GameObject healthPrefab;
    public GameObject manaPrefab;
    public Transform player;
    public Collider2D spawnArea;

    public float minOffset = 5f;  
    public float maxOffset = 25f; 

    public float spawnInterval = 5f;

    void Start()
    {
        SpawnPotion();

    }

    public void SpawnPotion()
    {
        Bounds bounds = spawnArea.bounds;

        float randomX = Random.Range(player.position.x + minOffset, player.position.x + maxOffset);

        randomX = Mathf.Clamp(randomX, bounds.min.x, bounds.max.x);

        float rayStartY = bounds.max.y;
        float rayEndY = bounds.min.y;

        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(randomX, rayStartY),
            Vector2.down,
            rayStartY - rayEndY,
            LayerMask.GetMask("Ground") 
        );

        if (hit.collider != null)
        {
            Vector2 spawnPos = hit.point + Vector2.up * 0.5f;

            GameObject prefab = Random.value < 0.5f ? healthPrefab : manaPrefab;

            if (prefab == null)
            {
                Debug.LogError("Prefab bi thieu!");
                return;
            }

            Instantiate(prefab, spawnPos, Quaternion.identity);

        }
    }
}
