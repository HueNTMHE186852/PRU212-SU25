using UnityEngine;

public class IncinerationDamageZone : MonoBehaviour
{
    private float lastDamageTime = -99f;
    private float exitTimer = 0f;
    private bool isPlayerInside = false;
    private Player1 player;

    public float tickInterval = 1f;
    public float resetAfter = 2f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponentInParent<Player1>();
            isPlayerInside = true;
            exitTimer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            exitTimer = 0f;
        }
    }

    void Update()
    {
        if (player == null) return;

        if (isPlayerInside)
        {
            if (Time.time >= lastDamageTime + tickInterval)
            {
                int damage = Mathf.RoundToInt(player.maxHealth * 0.1f);
                if (!player.isDefending)
                {
                    player.TakeDamage(damage);
                }
                lastDamageTime = Time.time;
            }
            exitTimer = 0f;
        }
        else
        {
            exitTimer += Time.deltaTime;
            if (exitTimer > resetAfter)
            {
                lastDamageTime = -99f; // reset damage timer
            }
        }
    }
}
