using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTrigger : MonoBehaviour
{
    public int damage = 20;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player1 player = collision.GetComponentInParent<Player1>();
            player.TakeDamage(damage);
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.05f));
            }
            Debug.Log("💥 Player trúng đòn lazer trừ 20 dame ");
        }
    }
}
