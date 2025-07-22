using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTrigger : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("⚡ Laser hit: " + collision.name + " (Tag: " + collision.tag + ")");

        if (collision.CompareTag("Defend"))
        {
            Debug.Log("🛡️ Hit Defend — Laser blocked.");
            return;
        }

        if (collision.CompareTag("Player"))
        {
            Player1 player = collision.GetComponentInParent<Player1>();
            AuronPlayerController auronPlayer = collision.GetComponentInParent<AuronPlayerController>();

            if (player == null && auronPlayer == null)
            {
                return;
            }

            if (player != null && player.isDefending)
            {
                Debug.Log("🛡️ Player is defending — no laser damage.");
                return;
            }
            if (auronPlayer != null && auronPlayer.isDefending)
            {
                Debug.Log("🛡️ AuronPlayer is defending — no laser damage.");
                return;
            }

            if (auronPlayer != null)
            {
                auronPlayer.TakeDamage(damage);
            }
            else
            {
                player.TakeDamage(damage);
            }

            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.05f));
            }
            else
            {
                Debug.LogWarning("⚠️ CameraShake.Instance is null.");
            }
        }
    }

}
