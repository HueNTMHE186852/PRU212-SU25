using UnityEngine;

public class DefendCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"🛡️ DefendCollider triggered by: {collision.name}");

        if (collision.CompareTag("EnemyAttack"))
        {
            // Block or reduce damage here
            Debug.Log("💥 Blocked enemy attack!");
        }
    }
}
