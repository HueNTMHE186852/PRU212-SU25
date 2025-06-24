using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDame : MonoBehaviour
{
    public float damage = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🔍 OnTriggerEnter2D with: {other.name}, Tag: {other.tag}");

        if (other.CompareTag("Defend"))
        {
            Debug.Log("🛡️ Hit Defend collider — attack blocked!");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Player1 player1 = other.GetComponent<Player1>();

            if (player1 == null)
            {
                Debug.LogWarning("⚠️ Player script (Player1) not found on the collided object.");
                return;
            }

            if (!player1.isDefending)
            {
                Debug.Log($"💥 Player hit for {damage} damage!");
                player1.TakeDamage((int)damage);

                if (CameraShake.Instance != null)
                {
                    Debug.Log("📸 Camera shake triggered.");
                    StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.1f));
                }
                else
                {
                    Debug.LogWarning("⚠️ CameraShake.Instance is null.");
                }
            }
            else
            {
                Debug.Log("🛡️ Player is defending — no damage taken.");
            }
        }
    }
}
