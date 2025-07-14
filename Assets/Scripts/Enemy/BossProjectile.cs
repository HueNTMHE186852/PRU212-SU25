using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float lifetime = 3f;
    [SerializeField] private GameObject explosionEffect;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {

        direction = dir.normalized;
        Destroy(gameObject, lifetime);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    private void SpawnHitEffect()
    {
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f); // Tự huỷ sau 1 giây
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("🟡 Va chạm với: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            bool tookDamage = false;

            AuronPlayerController auronPlayer = collision.GetComponentInParent<AuronPlayerController>();
            if (auronPlayer != null)
            {
                auronPlayer.TakeDamage(damage);
                Debug.Log("💥 Gây " + damage + " sát thương cho AuronPlayerController");
                tookDamage = true;
            }

            Player1 player = collision.GetComponentInParent<Player1>();
            if (player != null)
            {
                if (player.isDefending)
                {
                    Debug.Log("🛡️ Player is defending — no laser damage.");
                    return;
                }
                player.TakeDamage(damage);
                Debug.Log("💥 Gây " + damage + " sát thương cho Player1");
                tookDamage = true;
            }

            if (!tookDamage)
            {
                Debug.Log("❌ Không tìm thấy AuronPlayerController hoặc Player1 trên Player");
            }

            // ✅ Gọi hiệu ứng tan biến và huỷ đạn
            SpawnHitEffect();
            Destroy(gameObject);
        }
    
    }


}

