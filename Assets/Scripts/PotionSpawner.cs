using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    public GameObject healthPotionPrefab;
    public GameObject manaPotionPrefab;
    public Transform player;

    public float minDistance = 2.5f;
    public float maxDistance = 4f;

    private Player1 playerScript;
    private float lastSpawnX = float.MinValue;
    private GameObject currentPotion;

    void Start()
    {
        playerScript = player.GetComponent<Player1>();
        SpawnPotion();
    }

    public void SpawnPotion()
    {
        if (player == null || playerScript == null) return;

        float dir = playerScript.transform.localScale.x < 0 ? -1f : 1f;
        float offsetX = Random.Range(minDistance, maxDistance);
        float spawnX = player.position.x + dir * offsetX;

        if (Mathf.Abs(spawnX - lastSpawnX) < 1f) return;

        float groundY = player.position.y - 0.5f;
        Vector2 spawnPos = new Vector2(spawnX, groundY);

        GameObject prefab = Random.value < 0.5f ? healthPotionPrefab : manaPotionPrefab;
        currentPotion = Instantiate(prefab, spawnPos, Quaternion.identity);

        var pickup = currentPotion.GetComponent<PotionPickup>();
        if (pickup != null)
        {
            pickup.spawner = this;
        }

        lastSpawnX = spawnX;
    }

    public void OnPotionCollected()
    {
        SpawnPotion();
    }
}
