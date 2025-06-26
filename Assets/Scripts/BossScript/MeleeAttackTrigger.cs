using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackTrigger : MonoBehaviour
{
    public int damage = 15;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Defend"))
        {
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
