using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            hasHit = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            animator.SetTrigger("StickToEnemy");

            // Đặt mũi tên vào đúng điểm va chạm
            if (collision.contacts.Length > 0)
            {
                Vector2 hitPoint = collision.contacts[0].point;
                transform.position = hitPoint;
            }

            transform.parent = collision.transform;
            BossAI boss = collision.gameObject.GetComponent<BossAI>();
            if (boss != null)
            {
                // Knockback chỉ theo trục X, không có thành phần Y
                float direction = collision.transform.position.x > transform.position.x ? 1f : -1f;
                float knockbackForce = 5f;
                Vector2 knockback = new Vector2(direction * knockbackForce, 0f);
                boss.ApplyKnockback(knockback);

                // Tắt collider của arrow để tránh va chạm lặp lại
                GetComponent<Collider2D>().enabled = false;
            }

        }
    }


}
