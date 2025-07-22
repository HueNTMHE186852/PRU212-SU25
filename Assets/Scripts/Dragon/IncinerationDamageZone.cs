using UnityEngine;

public class IncinerationDamageZone : MonoBehaviour
{
    private float lastDamageTime = -99f;
    private float exitTimer = 0f;
    private bool isPlayerInside = false;

    private Player1 player1;
    private AuronPlayerController player2;

    public float tickInterval = 1f;
    public float resetAfter = 2f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Thử tìm Player1
            var p1 = other.GetComponentInParent<Player1>();
            if (p1 != null)
            {
                player1 = p1;
                isPlayerInside = true;
                exitTimer = 0f;
                return;
            }

            // Thử tìm AuronPlayerController
            var p2 = other.GetComponentInParent<AuronPlayerController>();
            if (p2 != null)
            {
                player2 = p2;
                isPlayerInside = true;
                exitTimer = 0f;
                return;
            }
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
        if (!isPlayerInside)
        {
            exitTimer += Time.deltaTime;
            if (exitTimer > resetAfter)
            {
                lastDamageTime = -99f;
            }
            return;
        }

        if (Time.time >= lastDamageTime + tickInterval)
        {
            if (player1 != null && !player1.isDefending)
            {
                int damage = Mathf.RoundToInt(player1.maxHealth * 0.1f);
                player1.TakeDamage(damage);
            }

            if (player2 != null && !player2.isDefending)
            {
                int damage = Mathf.RoundToInt(player2.maxHealth * 0.1f);
                player2.TakeDamage(damage);
            }

            lastDamageTime = Time.time;
        }
    }
}
