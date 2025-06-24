using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTrigger : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("⚡ Laser hit: " + collision.name + " (Tag: " + collision.tag + ")");

        // Blocked by defend collider
        if (collision.CompareTag("Defend"))
        {
            Debug.Log("🛡️ Hit Defend — Laser blocked.");
            return;
        }

        // Try to damage Player
        if (collision.CompareTag("Player"))
        {
            Player1 player = collision.GetComponentInParent<Player1>();
            if (player == null)
            {
                return;
            }

            if (player.isDefending)
            {
                Debug.Log("🛡️ Player is defending — no laser damage.");
                return;
            }

            // Apply damage
            player.TakeDamage(damage);

            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(0.1f, 0.05f));
            }
            else
            {
                Debug.LogWarning("⚠️ CameraShake.Instance is null.");
            }
        }
    }
}
